#!/bin/bash
docker rm -f ukc-client
# docker run  --env EP_HOST_NAME=ep --name ukc-client \
#             --env UKC_SO_PASSWORD=Unbound1! \
#             --add-host <host name>:<host ip> \
#             -i unboundukc/ukc-client:demo-java-encrypt

docker run --name ukc-client \
        --network=vhsm_unbound \
        --env-file=env \
        -t -i unboundukc/ukc-client:demo-java-encrypt

 #command to run docker image
#docker run -dit --env EP_HOST_NAME=ep1:8443 --env UKC_PARTITION:test --env UKC_SO_PASSWORD:Unbound1! --env UKC_PASSWORD:Unbound1! vsc-microsoft-365-dke-e97c3a9132f476c39eebfcec6a51c345 /root/data/start.sh