# Unbound CORE Integration with Skopeo and Portieris

This integration uses Unbound CORE to sign a container and then Skopeo and Portieris to verify the signature before running the container.

The integration is described using 2 devices:

1. Management Device - used to configure the system.
1. Signing Device - used to sign containers.

The integration uses these tools:

1. [Skopeo](https://github.com/containers/skopeo) - a container copying and signing command line tool.
1. [Portieris](https://github.com/IBM/portieris) - a Kubernetes service that blocks unauthorized containers from starting.
1. [Unbound CORE](https://www.unboundsecurity.com/) - Unbound's key management tool.

## Integration Overview

All 3 tools must be configured to work together. 

First, the following steps are executed one time.
1. Generate an RSA key pair used to sign containers in Unbound CORE.
1. Export the public pair of the signing key.
1. Create a secret in Kubernetes out of public key.
1. Configure ``skopeo`` to work with Unbound CORE.
1. Generate ``portieris`` template to check all starting containers for signatures in the NGINX server.
1. Start ``portieris`` and check that everything is working and container signatures are enforced.

Second, the following steps are executed for every container.
1. Sign a container and generate the signature file using the ``skopeo`` command.
1. Copy the generated signature file to the ``NGINX`` web server used to serve static files.

## Prerequisites
You need to have a running Unbound CORE and generate an RSA key that will be used for signing.


## On the Management Device

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

4. Edit ```portieris/templates/policies.yaml```.

    Change the last policy to be similiar to:

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

    The ```storeURL``` should point to the webserver used to host container signature. For example, we use **nginx**.

5. Install the kubernetes portieris service. It blocks all unsigned containers.
    ```
    helm delete -n portieris portieris || true
    helm install portieris --create-namespace --namespace portieris ./portieris --set IBMContainerService=false --debug
    ```


## On the Signing Device
This step can be done with a Docker container.

1. Install any missing packages. For example for CentOS 8:
    ```
    yum -y install jq less vim openssl java java-devel wget gpg
    RUN dnf -y install skopeo
    ```

2. Upload the *ekm-client.rpm* file to the device.

3. Setup the CORE ekm-client.
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

3. Export public key in pgp format and copy it to the device used to manage Kubernetes.

    ```
    gpg2 --armor --output /tmp/public.gpg --export $KEY_NAME
    scp /tmp/public.gpg management-box:
    ```

Next, for each container, sign it and push it to the repository (using Skopeo).

By default, on **CentOS 8** the container signatures are saved to the following directory: ```/var/lib/containers/sigstore```.
You need to copy the signatures directory to the **management device**.

```
PUBID=$(gpg --list-keys 2>/dev/null | sed -n -e  's/^ * //p')
skopeo copy --sign-by $PUBID docker://docker.io/library/busybox:latest \
  docker://docker.io/stremovsky/busybox:latest --override-os linux --dest-creds stremovsky:PASSWORD
scp /var/lib/containers/sigstore management-box:
```


## Test the System

1. Run a container that we already signed. It runs without any issues.
    ```
    kubectl run -i --tty busybox --image=stremovsky/busybox:latest --restart=Never -- sh
    ```
1. Run an unsigned container. It is blocked from running.
    ```
    kubectl run -i --tty busybox --image=library/busybox:latest --restart=Never -- sh
    ```

    Sample response:
    ```
    Error from server: admission webhook "trust.hooks.securityenforcement.admission.cloud.ibm.com" denied the request:
    simple: policy denied the request: A signature was required, but no signature exists
    ```
    
