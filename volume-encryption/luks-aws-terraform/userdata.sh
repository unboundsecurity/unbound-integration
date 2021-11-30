#!/bin/bash

set -x

yum install -y cryptsetup jq

#DEVICE=/dev/$(lsblk -n | awk '$NF != "/" {print $1}')
DEVICE=$(lsblk  --noheadings --raw | awk '{print substr($0,0,7)}' | uniq -c | grep ' 1' | awk '{print "/dev/"$2}')
FS_TYPE=$(file -s $DEVICE | awk '{print $2}')
MOUNT_POINT=/data

curl -k '${EP_URL}' -u '${EP_USER_PWD}' | jq -r '.keyData' > /root/keyfile
chmod 600 /root/keyfile

# If no FS, then this output contains "data"
if [ "$FS_TYPE" = "data" ]
then    
    #head -c 256 /dev/random > /root/keyfile
    cryptsetup -q luksFormat $DEVICE --key-file /root/keyfile
    cryptsetup luksOpen $DEVICE c1 --key-file /root/keyfile
    echo "Creating file system on $DEVICE"
    mkfs -F -t ext4 /dev/mapper/c1
else
    cryptsetup luksOpen $DEVICE c1 --key-file /root/keyfile
fi

echo "c1 $DEVICE /root/keyfile luks,discard" >> /etc/crypttab
UUID=`lsblk /dev/mapper/c1 -o UUID -n`
echo "/dev/mapper/c1 /data                        ext4    defaults,nofail 0 2" >> /etc/fstab
cryptsetup -v luksDump /dev/mapper/c1

mkdir -p $MOUNT_POINT
mount /dev/mapper/c1 $MOUNT_POINT
