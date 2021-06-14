#!/bin/bash
set -e

echo "UKC ready registering client"

PARTITION=$UKC_PARTITION
CLIENT="${UKC_CLIENT_NAME:-ephemeral}"
EP_HOST_NAME="${EP_HOST_NAME:-ep}"
echo $CLIENT_TEMPLATE_ACTIVATION_CODE
ACTIVATION_CODE=$CLIENT_TEMPLATE_ACTIVATION_CODE

ucl register --template $CLIENT_TEMPLATE_NAME -p $PARTITION  -c $ACTIVATION_CODE

echo "Client registered"
