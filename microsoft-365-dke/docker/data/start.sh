#!/bin/bash
set -x

export PORT=8080

export ASPNETCORE_URLS=http://*:$PORT

echo "servers=$EP_HOST_NAME">/etc/ekm/client.conf

echo "$UKC_SERVER_IP ep1" >> /etc/hosts

# Wait until UKC is ready
sh /root/data/wait_for_ukc_cluster_to_start.sh

sh /root/data/create_partition.sh
# Register UKC client - establish secure connection with PKCS11 
#sh /root/data/register_new_client_ephemeral.sh
sh /root/data/register_new_client.sh
cd /root/data/publish

service ssh start

dotnet unboundkeystore.dll