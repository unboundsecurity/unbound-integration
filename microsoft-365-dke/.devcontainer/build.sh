#!/bin/bash
docker build \
--build-arg UKC_CLIENT_INSTALLER_URL=/root/data/ekm-client-2.0.2010.38445-el7+el8.x86_64.rpm \
-t unboundukc/ukc-client:demo-java-encrypt \
-f ./Dockerfile ..


