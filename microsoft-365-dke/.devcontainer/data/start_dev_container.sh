#!/bin/bash

# This script is used in the postCreateCommand of the Visual Studio Code Dev Container
set -e

# Wait until UKC is ready
/root/data/wait_for_ukc_cluster_to_start.sh

# Register UKC client - establish secure connection with PKCS11 
/root/data/register_new_client.sh
