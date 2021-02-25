#!/bin/bash
docker-compose down
docker-compose up -d
rm -rf ./store
node app.js --crypto-suite pkcs11 setup

echo
echo "About to delete all UKC keys from partition 'fabric'"
read -p "Are you sure? [y/n] " -n 1 -r
echo    
if [[ $REPLY =~ ^[Yy]$ ]]
then	
  ucl list | grep -Po 'UID=\K\w+' | while read -r a; do ucl delete -u $a -w UserPassword1! -p fabric; done
fi
