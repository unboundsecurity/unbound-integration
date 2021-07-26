#!/bin/bash

install_url="https://repo.dyadicsec.local/cust/autotest/ekm/2.0.2103.39708/linux/ekm-client_2.0.2103.39708.deb9_amd64.deb"
tag="unboundukc/ms-dke-service:latest"

docker build -t $tag --no-cache \
--build-arg UKC_CLIENT_INSTALLER_URL=$install_url \
$(dirname "$0")

