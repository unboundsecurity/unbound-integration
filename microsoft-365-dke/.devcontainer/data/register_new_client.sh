#!/bin/bash
set -e

echo "UKC ready registering client"

PARTITION=$UKC_PARTITION
CLIENT="${UKC_CLIENT_NAME:-encrypt_demo}"
EP_HOST_NAME="${EP_HOST_NAME:-ep}"

echo "Deleting client $CLIENT from $PARTITION"

# Delete if exists
curl "https://$EP_HOST_NAME/api/v1/clients/$CLIENT?partitionId=$PARTITION" \
  -X DELETE -H 'Connection: keep-alive' \
  -H 'Accept: application/json, text/plain, */*' \
  --fail \
  --user "so@$UKC_PARTITION:$UKC_PASSWORD" --compressed --insecure --output /dev/null --silent || true

echo "Creating client $CLIENT from $UKC_PARTITION"
ACTIVATION_CODE=$(curl "https://$EP_HOST_NAME/api/v1/clients?partitionId=$PARTITION" \
 -H 'Connection: keep-alive' \
 -H 'Accept: application/json' \
 --user "so@$UKC_PARTITION:$UKC_PASSWORD" \
 -H 'Content-Type: application/json' \
 --compressed --insecure --data-binary "{\"name\":\"$CLIENT\"}")

ACTIVATION_CODE=$(echo $ACTIVATION_CODE | jq -r '.activationCode')

echo "Activation code is $ACTIVATION_CODE"
ucl register -p $PARTITION -n $CLIENT -c $ACTIVATION_CODE
#ucl register -p $PARTITION --template template1 -c 5751545082815582

echo "Client registered"
