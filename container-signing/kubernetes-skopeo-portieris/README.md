# Unbountsecurity vHSM integration with Skopeo and Portieris

## Project Components

[Skopeo](https://github.com/containers/skopeo) is a container copying and signing command.

[Portieris](https://github.com/IBM/portieris) is a Kubernetes service that blocks unauthorized containers from starting.

[Unboundsecurity VHSM](https://www.unboundsecurity.com/) is a virtual HSM service.

## The process

Project task is to integrate all 3 components to work together. It is done as following:
1. Generate an RSA key pair used to sign containers in Unboundsecurity HSM.
1. Export the public pair of the signing key.
1. Create a secret in Kubernetes out of public key.
1. Configure ``skopeo`` to work with Unboundsecurity HSM service.
1. Sign container and generate the signature file using the ``skopeo`` command.
1. Copy generated signature file to the ``NGINX`` web server used to serve static files.
1. Generate ``portieris`` template to check all starting containers for signatures in NGINX server.
1. Start ``portieris`` and check that everything is working and container signatures enforced.

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

4. Sign a public container and push it to the repository.

By default, on **CentOS 8** the container signatures will be saved to the following directory: ```/var/lib/containers/sigstore```.
You need to copy the signatures directory. For example to the **management machine**.

```
PUBID=$(gpg --list-keys 2>/dev/null | sed -n -e  's/^ * //p')
skopeo copy --sign-by $PUBID docker://docker.io/library/busybox:latest \
  docker://docker.io/stremovsky/busybox:latest --override-os linux --dest-creds stremovsky:PASSWORD
scp /var/lib/containers/sigstore management-box:
```

## On the management machine.

1. Create a Kubernetes secret form the ```public.gpg``` file.
```
kubectl create secret generic signing-pubkey --from-file=key=public.gpg
```

2. Start the **nginx** container to serve static signatures.

```
docker run --rm  -v `pwd`/sigstore:/usr/share/nginx/html --name nginx -p 1280:80 -d nginx
```

3. Download and prepare portieris as in https://github.com/IBM/portieris

```
sh ./portieris/gencerts
```

4. Edit ```portieris/templates/policies.yaml``` file.

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

The ```storeURL``` should point to the webserver used to host container signatured. For example we use **nginx**.

5. Instal the kubernetes portieris service. It will block now all unsigned containers.

```
helm delete -n portieris portieris || true
helm install portieris --create-namespace --namespace portieris ./portieris --set IBMContainerService=false --debug
```

## Test that everything is setup correctly.

This command must work. We run conatuner that we have already signed.
```
kubectl run -i --tty busybox --image=stremovsky/busybox:latest --restart=Never -- sh
```

This command must be blocked. We run unsigned container.
```
kubectl run -i --tty busybox --image=library/busybox:latest --restart=Never -- sh
```
