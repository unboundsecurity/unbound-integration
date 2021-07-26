#!/bin/bash
set -e

echo "UKC ready registering client"

ucl register --template $CLIENT_TEMPLATE_NAME -p $UKC_PARTITION  -c $CLIENT_TEMPLATE_ACTIVATION_CODE

echo "Client registered"
