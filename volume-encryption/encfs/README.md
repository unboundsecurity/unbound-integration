# Volume encryption with EncFS

## Overview
Protecting sensitive files with Unbound MPC technology is possible using [EncFS](https://en.wikipedia.org/wiki/EncFS).

EncFS provides transparent file encryption for an arbitrary volume and is supported by many platforms. The volume can be mounted to any media supported by the host including cloud volumes like [Amazom EBS](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ebs-using-volumes.html) 

Files are encrypted using a volume key and a password is used to decrypt that key.

This guide provides instructions and scripts for using Unbound MPC technology to protect the volume encryption key password. 
This provides the following benefits:
* Strong password protection with MPC technology
* Key rotation for increased security 

## Prerequisites
1. A working Unbound CORE KMS cluster
2. The client machine should have
   1. EncFS package installed
   2. For communications with Unbound CORE Server, either of the two options:
      1. For communications using [CORE REST API](http://www.unboundsecurity.com/docs/ukc_rest/ukc.html) 
         For this option **curl** should be installed  
         This option is easier to deploy but may be less secure
      1. For communications using Unbound CORE Client, The CORE Client package should be installed
         This option is more secure but requires [installation and configuration of Unbound CORE Client](https://www.unboundsecurity.com/docs/UKC/UKC_Installation/Content/Products/UKC-EKM/UKC_User_Guide/UG-Inst/ClientInstallation.html)
## Prepare the encryption key
The first step is to [create an RSA key with Unbound CORE KMS](https://www.unboundsecurity.com/docs/UKC/UKC_Interfaces/Content/Products/UKC-EKM/UKC_User_Guide/UG-If/uiSO/KeyTab.html#Create2).
This key will be used for encrypting the EncFS volume password.
Unbound KMS stores keys in segregated containers called Partitions so the first step is to [create and configure a partition](https://www.unboundsecurity.com/docs/UKC/UKC_Interfaces/Content/Products/UKC-EKM/UKC_User_Guide/UG-If/uiRoot/PartitionsTab.html#Create) for our key. This step can be skipped if you already have an existing partition you want to use.

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
## Start EncFS 
Start EncFS Daemon using the password encrypted with Unbound CORE 
```
encfs --extpass="ub-get-password" --standard /your-encrypted-data-folder /your-mounted-data-folder
```

## Key rotation

## Demo with docker
The solution described here can be easily demonstrated on a docker container.

```
FROM ubuntu:18.04

```

