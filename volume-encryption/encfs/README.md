# Volume encryption with EncFS

## Overview
Protecting sensitive files with Unbound MPC technology is possible by using EncFS.

[EncFS](https://en.wikipedia.org/wiki/EncFS) provides transparent file encryption for an arbitrary volume and is supported by many platforms. The volume can be mounted to any media supported by the host including cloud volumes like [Amazom EBS](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ebs-using-volumes.html) 

Files are encrypted using a volume key and a password is used to decrypt that key.

This guide provides instructions and scripts for using Unbound MPC technology to protect the volume key password. This provides the following benefits:
* Strong protection with MPC technology
* Key rotation for increased security 

## Prerequisites
1. A working Unbound CORE KMS cluster
2. EncFS installed
3. curl 

## Prepare the encryption key
The first step is to create an RSA key with Unbound CORE KMS.
This key will be used for encrypting the EncFS volume password.
Unbound KMS stores keys in segregated containers called Partitions so the first step is to create and configure a partition for our key. This step can be skipped if you already have an existing partition you want to use.

The following preparation steps in this section should be carried out by an Unbound CORE Administrator (SO) user. It can be done from Unbound CORE Web Admin (UI) or by using a script with unbound CLI, called `ucl` as follows:

```
# Set the partition name - you may change it or use existing partition
PARTITION_NAME=encfs 
# Set the partition administrator(usename so) password
PARTITION_ADMIN_PASSWORD=<use a strong password> 
# Create the partition if necessary
sudo ucl partition create -p $PARTITION_NAME -w Password1! -s Password1

# Create an RSA key for encryption. You may change the key name.
ENCFS_KEY_NAME=encfskey
ucl generate -t RSA -p $PARTITION_NAME --name $ENCFS_KEY_NAME

# Create a partition user with restricted permissions only for crypto operations.
PARTITION_CRYPTO_USER_NAME=encfs_user
PARTITION_CRYOTO_USER_PASSWORD=<replace with a strong password>
ucl user create -p $PARTITION_NAME -n $PARTITION_CRYPTO_USER_NAME -d $PARTITION_CRYOTO_USER_PASSWORD -w PARTITION_ADMIN_PASSWORD -r user
```
## Run the script
```
encfs --extpass="ubound_encfs.sh" /your-encrypted-data-folder /your-decrypted-data-folder
```
## Generate and encrypt the password
```
head -c 128 /dev/random | openssl rsautl -encrypt -pubin -inkey ./cert.pub -oaep | base64
```


```
encfs /folder-with-encrypted-data /folder-with-clear-data --extpass="ucl decrypt -i /data/keyphase.enc -o /dev/stdout"'
```

## Key rotation

## Demo with docker
The solution described here can be easily demonstrated on a docker container

```
FROM ubuntu:18.04

```

