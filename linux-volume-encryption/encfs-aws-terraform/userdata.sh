#!/bin/bash

DEVICE=$(lsblk  --noheadings --raw | awk '{print substr($0,0,7)}' | uniq -c | grep ' 1' | awk '{print "/dev/"$2}')
mkfs -t ext4 $DEVICE
RESULT=$?

for ((n=0; n<60 && $RESULT != 0; n++))
do
  DEVICE=$(lsblk  --noheadings --raw | awk '{print substr($0,0,7)}' | uniq -c | grep ' 1' | awk '{print "/dev/"$2}')
  mkfs -t ext4 $DEVICE
  RESULT=$?
  if [ $RESULT != 0 ]; then
     echo "error in[$n]: DEVICE"
     sleep 1
  fi
done

set -x

mkdir /data
echo $DEVICE' /data       ext4     defaults,nofail,noatime,barrier=0,data=writeback     0 2' >> /etc/fstab
mount -a
df

#yum install -y wget fuse jq openssl
yum install -y wget fuse
sudo dnf -y --enablerepo=powertools install tinyxml2

wget --no-verbose https://download-ib01.fedoraproject.org/pub/epel/8/Everything/x86_64/Packages/f/fuse-encfs-1.9.5-5.el8.x86_64.rpm -O /tmp/fuse-encfs.rpm
rpm -i /tmp/fuse-encfs.rpm

${WGET_EKM_CLIENT}
rpm -i /tmp/ekm-client.rpm

if [ ! -z ${EP_HOST_IP} ]; then
  echo "${EP_HOST_IP} ${EP_HOST_NAME}" >> /etc/hosts
fi
echo "servers=${EP_HOST_NAME}" > /etc/ekm/client.conf

ucl register -t ${CLIENT_NAME} -p ${PARTITION} -c ${ACTIVATION_CODE}
ucl list

# set up openssl to work with Unboundsecurity engine
#yes | /etc/ekm/dy_openssl

# prepare encrypted keyphase on other host - for example on Terraform
# ucl export -n aaa --output cert.pub
# head -c 128 /dev/random | openssl rsautl -encrypt -pubin -inkey ./cert.pub -oaep | base64

echo ${ENC_KEYPHASE} | base64 -d > /data/keyphase.enc
chown centos:centos /data/keyphase.enc
mkdir -p /data/{clear,encrypted}
chown -R centos:centos /data/

#sudo su centos -c 'echo | encfs /data/encrypted /data/clear --extpass="cat /data/keyphase.b64 | base64 --decode | openssl rsautl -decrypt -inkey ${RSA_KEY_NAME} -engine dyadicsec -keyform engine -oaep"'
#ucl decrypt -i /data/keyphase.enc -o /data/keyphase
sudo su centos -c 'echo | encfs /data/encrypted /data/clear --extpass="ucl decrypt -i /data/keyphase.enc -o /dev/stdout"'
