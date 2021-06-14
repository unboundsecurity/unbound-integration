#!/bin/bash
set -e

EP_HOST_NAME="${EP_HOST_NAME:-ep}"

echo "Waiting for EP, Partner, Aux ping"
#until ping -c1 ukc-ep &>/dev/null; do :; done
#until ping -c1 ukc-partner &>/dev/null; do :; done
#until ping -c1 ukc-aux &>/dev/null; do :; done

echo "Waiting for EP to start"
until $(curl --output /dev/null -k --silent --head --fail \
  https://${EP_HOST_NAME}/api/v1/health); do
    printf '*'
    sleep 5
done

# echo "Waiting for partition '${UKC_PARTITION}' 'so' password reset"
# until $(curl --output /dev/null --silent -k --fail --compressed \
#   --user "so@$UKC_PARTITION:$UKC_PASSWORD" \
#   "https://${EP_HOST_NAME}/api/v1/info?partitionId=$UKC_PARTITION" ); do
#     printf '.'
#     sleep 5
# done

echo "UKC Ready"