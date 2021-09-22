#!/bin/bash
set -e

export PORT=8080

export ASPNETCORE_URLS=http://*:$PORT

echo "check if all needed application settings are set:"
required_vars=(EP_HOST_NAME UKC_PARTITION UKC_SERVER_IP)

missing_vars=()
for i in "${required_vars[@]}"
do
    test -n "${!i:+y}" || missing_vars+=("$i")
done
if [ ${#missing_vars[@]} -ne 0 ]
then
    echo "The following variables are not set, but should be:" >&2
    printf ' %q\n' "${missing_vars[@]}" >&2
    exit 1
fi


echo "servers=$EP_HOST_NAME">/etc/ekm/client.conf

echo "$UKC_SERVER_IP ep1" >> /etc/hosts

# Wait until UKC is ready
sh /root/data/wait_for_ukc_cluster_to_start.sh

cd /root/data/publish

dotnet unboundkeystore.dll
