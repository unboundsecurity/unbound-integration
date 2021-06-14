#!/bin/bash
# set -e

UKC_SO_PASSWORD="${UKC_SO_PASSWORD:-Unbound1!}"
EP_HOST_NAME="${EP_HOST_NAME:-ep}"

status_code=$(curl --write-out '%{http_code}' --silent --output /dev/null -k \
         --user "so@root:Unbound1!" "https://$EP_HOST_NAME/api/v1/partitions/$UKC_PARTITION")
if [[ "$status_code" -ne 200 ]] ; then
  echo "Partition '$UKC_PARTITION' not found";
  read -p "Create partition '$UKC_PARTITION'?" -n 1 -r
  echo 
  if [[ $REPLY =~ ^[Yy]$ ]]
  then
      status_code=$(curl --write-out '%{http_code}'  "https://$EP_HOST_NAME/api/v1/partitions" \
      -H 'Accept: application/json' \
      --user "so@root:$UKC_SO_PASSWORD" \
      -H 'Content-Type: application/json' \
      --data-raw $"{\"name\":\"$UKC_PARTITION\",\"soPassword\":\"$UKC_SO_PASSWORD\",\"inherit\":false,\"propagate\":false,\"fipsRequirements\":\"FIPS_NONE\",\"allowKeystores\":false,\"newClient\":{\"name\":\"a\"}}" \
      --compressed \
      --silent \
      --insecure --output /dev/null)
      if [[ "$status_code" -ne 201 ]] ; then
        echo "Could not create partition"
      else
        echo "Partition '$UKC_PARTITION' created successfully."
      fi
  else
    exit 1
  fi
else
  echo "Found partition '$UKC_PARTITION'."
fi
