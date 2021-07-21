#!/bin/bash
# This script is used in the postCreateCommand of the Visual Studio Code Dev Container
set -x

export PORT=8080

export ASPNETCORE_URLS=http://*:$PORT

# configure env params
# export EP_HOST_NAME="ep1"
# export UKC_PARTITION="test"
# export UKC_SO_PASSWORD="Unbound1!"
# export UKC_PASSWORD="Unbound1!"
# export UKC_SERVER_IP="54.174.121.27"

echo "servers=$EP_HOST_NAME">/etc/ekm/client.conf

echo "$UKC_SERVER_IP ep1" >> /etc/hosts

# Wait until UKC is ready
sh /root/data/wait_for_ukc_cluster_to_start.sh

sh /root/data/create_partition.sh
# Register UKC client - establish secure connection with PKCS11 
#sh /root/data/register_new_client_ephemeral.sh
sh /root/data/register_new_client.sh
cd /root/data/published

service ssh start

dotnet customerkeystore.dll

#tail -f /dev/null #keep container running