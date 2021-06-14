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