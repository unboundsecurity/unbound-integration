#!/bin/bash
set -e

# Common utilities and clean
echo 'alias ll="ls -l"' >> ~/.bashrc
    apt-get update -y 
    apt-get install curl 
    apt-get install -y policycoreutils-python-utils 
    apt-get install libssl-dev
    apt-get clean -y

# JQ - Json parser
curl -LO# https://github.com/stedolan/jq/releases/download/jq-1.6/jq-linux64; \
    mv ./jq-linux64 ./jq
    chmod +x ./jq
    mv jq /usr/bin    

echo "installing UKC client"
cd "/home/site/wwwroot/data"
dpkg -i ekm-client_2.0.2010.38476.deb9_amd64.deb
echo "UKC Client Installed successfully"

apt-get update 
apt-get install -y --allow-unauthenticated libc6-dev 
apt-get install -y --allow-unauthenticated libgdiplus 
apt-get install -y --allow-unauthenticated libx11-dev  
rm -rf /var/lib/apt/lists/*

# params for ukc
export EP_HOST_NAME="ep1"
export KC_CRYPTO_USER="encrypter"
export UKC_CRYPTO_USER_PASSWORD="Password1!"
export UKC_PARTITION="test"
export UKC_SO_PASSWORD="Unbound1!"
export UKC_PASSWORD="Unbound1!"

echo "servers=$EP_HOST_NAME">/etc/ekm/client.conf

echo "54.174.121.27 ep1" >> /etc/hosts

sh wait_for_ukc_cluster_to_start.sh
sh create_partition.sh
sh register_new_client.sh
cd "/home/site/wwwroot/"


##############

export PORT=8080

export ASPNETCORE_URLS=http://*:$PORT

echo Trying to find the startup DLL name...
echo Found the startup D name: customerkeystore.dll
echo 'Running the command: dotnet "customerkeystore.dll"'
dotnet "customerkeystore.dll"