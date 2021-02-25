#!/bin/bash
echo "About to delete all fabric user keys from UKC partition 'fabric'"
read -p "Are you sure? [y/n] " -n 1 -r
echo    
if [[ $REPLY =~ ^[Yy]$ ]]
then    
  ucl list | ucl list | grep -Po 'UID=\K\w+(?=\sName="user\d+)' | while read -r a; do ucl delete$
fi
