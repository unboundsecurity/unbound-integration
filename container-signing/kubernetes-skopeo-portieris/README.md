## Prerequisites
Generate RSA key in the Unbound UKC.

## Setup a signing machine.
It can be a Docker container.

1. Install missing packages. For example for CentOS 8:
```
yum -y install jq less vim openssl java java-devel wget gpg
RUN dnf -y install skopeo
```

2. Upload the ekm-client.rpm file to the machine.

3. Setup the ekm-client.
```
KEY_NAME="..."
PARTITION="..."
CLIENT_TEMPLATE="..."
UKC="server-name:port"
ACCESS_CODE="123..."
echo "servers="$UKC > /tmp/install_props.txt
export INSTALL_PROPS=/tmp/install_props.txt
yum localinstall -y /tmp/ekm-client-*.rpm
export RND=`< /dev/urandom LC_CTYPE=C tr -dc 'A-Za-z-0-9' | head -c${1:-32};`
ucl register -n client-$RND -t $CLIENT_TEMPLATE -p $PARTITION -c $ACCESS_CODE
ucl pgp-key -p $PARTITION -n $KEY_NAME
```

3. [One-time] Export public key in pgp format and copy it to the machine used to manage kubernetes.

```
gpg2 --armor --output /tmp/public.gpg --export $KEY_NAME
scp /tmp/public.gpg management-box:
```

On the Kubernetes management machine create a Kubernetes secret:
```
kubectl create secret generic signing-pubkey --from-file=key=public.gpg
```

4. Sign a public container and push it to the repository.
```
PUBID=$(gpg --list-keys 2>/dev/null | sed -n -e  's/^ * //p')
skopeo copy --sign-by $PUBID docker://docker.io/library/busybox:latest \
  docker://docker.io/stremovsky/busybox:latest --override-os linux --dest-creds stremovsky:PASSWORD
```

By default, on **CentOS 8** the container signatures will be saved to the following directory: ```/var/lib/containers/sigstore```.
You need to copy the whole directory. For example on the management machine.

On the management machine start an nginx container to serve static signatures.

```
docker run --rm  -v sigstore:/usr/share/nginx/html --name nginx -p 1280:80 -d nginx
```


## On the management machine.

1. Download and prepare portieris as in https://github.com/IBM/portieris

```
sh ./portieris/gencerts
```

2. Edit ```portieris/templates/policies.yaml``` file.

Change the last policy to be something like:

```
apiVersion: portieris.cloud.ibm.com/v1
kind: ClusterImagePolicy
metadata:
  name: default
  annotations:
    helm.sh/hook: post-install
    helm.sh/hook-weight: "1"
spec:
   repositories:
     - name: '*'
       policy:
         simple:
           storeURL: http://host.docker.internal:1280
           requirements:
             - type: "signedBy"
               keySecret: signing-pubkey
```

The ```storeURL``` will point to the webserver used to host container signatured.


helm delete -n portieris portieris

helm install portieris --create-namespace --namespace portieris ./portieris --set IBMContainerService=false --debug

## Run a test kubernetes container:
```
kubectl run -i --tty busybox --image=stremovsky/busybox:latest --restart=Never -- sh
```

