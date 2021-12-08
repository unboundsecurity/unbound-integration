#!/bin/bash

install_url="https://repo.dyadicsec.local/cust/autotest/ekm/2.0.2106.42245/linux/ekm-client_2.0.2106.42245.deb9_amd64.deb"
tag="unboundukc/ms-dke-service:latest"

# --no-cache
docker build -t $tag  \
--build-arg UKC_CLIENT_INSTALLER_URL=$install_url \
$(dirname "$0")

