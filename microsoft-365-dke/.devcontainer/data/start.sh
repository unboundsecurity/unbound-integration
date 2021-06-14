#!/bin/bash
# This script is used in the postCreateCommand of the Visual Studio Code Dev Container
set -e

echo "servers=$EP_HOST_NAME">/etc/ekm/client.conf

# Wait until UKC is ready
sh /root/data/wait_for_ukc_cluster_to_start.sh

sh /root/data/create_partition.sh
# Register UKC client - establish secure connection with PKCS11 
#sh /root/data/register_new_client_ephemeral.sh
 sh /root/data/register_new_client.sh

#
#sh /root/data/run_encrypt_demo.sh

# tail -f /dev/null #keep container running