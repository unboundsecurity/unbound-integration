#!/bin/bash

install_url="https://repo.dyadicsec.local/cust/autotest/ekm/2.0.2106.42245/linux/ekm-client_2.0.2106.42245.deb9_amd64.deb"
tag="unboundukc/ms-dke-service:latest"

# --no-cache
docker build -t $tag  \
--build-arg UKC_CLIENT_INSTALLER_URL=$install_url \
$(dirname "$0")

rm -rf $(pwd)/../src/unbound-key-store/bin
rm -rf $(pwd)/../src/unbound-key-store/obj

rm -rf ./data/publish
docker run --entrypoint="dotnet" -v $(pwd)/..:/unboundkeystore \
    $tag publish /unboundkeystore/src/unbound-key-store/unboundkeystore.csproj \
    --output /unboundkeystore/docker/data/publish \
    /property:GenerateFullPaths=true 

docker build -t $tag  \
--build-arg UKC_CLIENT_INSTALLER_URL=$install_url \
$(dirname "$0")

