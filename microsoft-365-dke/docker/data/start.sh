#!/bin/bash
set -e

export PORT=8080

export ASPNETCORE_URLS=http://+:$PORT

echo "check if all needed application settings are set:"
required_vars=(UB_CORE_URL UB_PARTITION UB_USER UB_USER_PASSWORD)

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

wait_for_ukc_cluster_to_start.sh

# Install UKC root CA certificate
if [ ! -z "$UB_CA_CERT_B64" ]; then
  base64 -d $UB_CA_CERT_B64 > /usr/local/share/ca-certificates/unbound-core-ca.pem
  cp /usr/local/share/ca-certificates/unbound-core-ca.pem /etc/ssl/certs
  update-ca-certificates --fresh
else
  ub-install-ca-certificate
fi

# now verify that TLS connection is working
echo Verify that TLS connection to Unbound CORE is working
echo This requires that the Unbound CORE EP certificate 
echo  contains the host name of $UB_CORE_URL and that its
echo  CA certificate is trusted
set -x
curl $UB_CORE_URL/api/v1/info

cd /unbound/publish

dotnet unboundkeystore.dll
