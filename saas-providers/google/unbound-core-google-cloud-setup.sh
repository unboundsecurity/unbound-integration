#!/bin/bash -xe

PQDN=lb22googlecloud
FQDN=${PQDN}.unboundtech.com
NETWORK_NAME=ekm-network
REGION_01=europe-west1
REGION_02=europe-west2
INSTANCE_GROUP_ZONE_01=${REGION_01}-c
INSTANCE_GROUP_ZONE_02=${REGION_02}-b
MACHINE_TYPE=n1-standard-2

EKM_VERSION=2.0.2007.54134
DISTRO=UBUNTU18
IMAGE_FAMILY=
IMAGE_PROJECT=
EKM_PKG=
URL_EKM_PKG=
STARTUP_SCRIPT=
case $DISTRO in
CENTOS7*)
  IMAGE_FAMILY=centos-7
  IMAGE_PROJECT=centos-cloud
  EKM_PKG=ekm-${EKM_VERSION}.el7.x86_64.rpm
  URL_EKM_PKG=https://repo.dyadicsec.com/cust/autotest/ekm/${EKM_VERSION}/linux/${EKM_PKG}
  STARTUP_SCRIPT="#! /bin/bash
   yum update
   yum install -y java-11-openjdk-headless telnet iputils nmap-ncat vim less bind-utils
   yum localinstall -y ${URL_EKM_PKG}
   "
  ;;
CENTOS8*)
  IMAGE_FAMILY=centos-8
  IMAGE_PROJECT=centos-cloud
  EKM_PKG=ekm-${EKM_VERSION}.el8.x86_64.rpm
  URL_EKM_PKG=https://repo.dyadicsec.com/cust/autotest/ekm/${EKM_VERSION}/linux/${EKM_PKG}
  STARTUP_SCRIPT="#! /bin/bash
   yum update
   yum install -y java-11-openjdk-headless telnet iputils nmap-ncat vim less bind-utils
   yum localinstall -y ${URL_EKM_PKG}
   "
  ;;
UBUNTU18*)
  IMAGE_FAMILY=ubuntu-minimal-1804-lts
  IMAGE_PROJECT=ubuntu-os-cloud
  EKM_PKG=ekm_${EKM_VERSION}.deb9_amd64.deb
  URL_EKM_PKG=https://repo.dyadicsec.com/cust/autotest/ekm/${EKM_VERSION}/linux/${EKM_PKG}
  STARTUP_SCRIPT="#! /bin/bash
   apt-get update
   apt-get install -y openjdk-11-jdk-headless telnet iputils-ping netcat vim less dnsutils 
   wget --no-check-certificate ${URL_EKM_PKG}
   dpkg -i ${EKM_PKG}
   "
  ;;
*)
  set +x
  echo Not supported Linux distribution \"$DISTRO\"
  set -x
  exit 1
  ;;
esac


########################################
# VPC
########################################
gcloud compute networks create ${NETWORK_NAME} --subnet-mode=custom

# Networks
gcloud compute networks subnets create ${REGION_01}-subnet \
  --network=${NETWORK_NAME} \
  --range=10.1.10.0/24 \
  --region=${REGION_01}

gcloud compute networks subnets create ${REGION_02}-subnet \
  --network=${NETWORK_NAME} \
  --range=10.1.11.0/24 \
  --region=${REGION_02}

########################################
# Firewall
########################################
gcloud compute firewall-rules create fw-lb-all --network ${NETWORK_NAME} --allow tcp,udp,icmp


########################################
# Instances
########################################
# 3
gcloud compute instance-templates create ep-${REGION_01}-template \
   --machine-type=${MACHINE_TYPE} \
   --region=${REGION_01} \
   --network=${NETWORK_NAME} \
   --subnet=${REGION_01}-subnet \
   --tags=allow-health-check \
   --image-family=${IMAGE_FAMILY} \
   --image-project=${IMAGE_PROJECT} \
   --metadata=startup-script="$STARTUP_SCRIPT
   "

gcloud compute instance-groups managed create ep-${REGION_01} \
   --template=ep-${REGION_01}-template --size=1 --zone=${INSTANCE_GROUP_ZONE_01}

# 4
gcloud compute instance-templates create ep-${REGION_02}-template \
   --machine-type=${MACHINE_TYPE} \
   --region=${REGION_02} \
   --network=${NETWORK_NAME} \
   --subnet=${REGION_02}-subnet \
   --tags=allow-health-check \
   --image-family=${IMAGE_FAMILY} \
   --image-project=${IMAGE_PROJECT} \
   --metadata=startup-script="$STARTUP_SCRIPT
   "

gcloud compute instance-groups managed create ep-${REGION_02} \
   --template=ep-${REGION_02}-template --size=1 --zone=${INSTANCE_GROUP_ZONE_02}

# 1
gcloud compute instance-templates create partner-${REGION_01}-template \
   --machine-type=${MACHINE_TYPE} \
   --region=${REGION_01} \
   --network=${NETWORK_NAME} \
   --subnet=${REGION_01}-subnet \
   --tags=allow-health-check \
   --image-family=${IMAGE_FAMILY} \
   --image-project=${IMAGE_PROJECT} \
   --metadata=startup-script="$STARTUP_SCRIPT
     "

gcloud compute instance-groups managed create partner-${REGION_01} \
   --template=partner-${REGION_01}-template --size=1 --zone=${INSTANCE_GROUP_ZONE_01}

# 2
gcloud compute instance-templates create partner-${REGION_02}-template \
   --machine-type=${MACHINE_TYPE} \
   --region=${REGION_02} \
   --network=${NETWORK_NAME} \
   --subnet=${REGION_02}-subnet \
   --tags=allow-health-check \
   --image-family=${IMAGE_FAMILY} \
   --image-project=${IMAGE_PROJECT} \
   --metadata=startup-script="$STARTUP_SCRIPT
    "

gcloud compute instance-groups managed create partner-${REGION_02} \
   --template=partner-${REGION_02}-template --size=1 --zone=${INSTANCE_GROUP_ZONE_02}

########################################
# Adding a named port to the instance group
########################################
gcloud compute instance-groups unmanaged set-named-ports partner-${REGION_01} \
    --named-ports https:443 \
    --zone ${INSTANCE_GROUP_ZONE_01}

gcloud compute instance-groups unmanaged set-named-ports ep-${REGION_01} \
    --named-ports https:443 \
    --zone ${INSTANCE_GROUP_ZONE_01}

gcloud compute instance-groups unmanaged set-named-ports partner-${REGION_02} \
    --named-ports https:443 \
    --zone ${INSTANCE_GROUP_ZONE_02}

gcloud compute instance-groups unmanaged set-named-ports ep-${REGION_02} \
    --named-ports https:443 \
    --zone ${INSTANCE_GROUP_ZONE_02}


########################################
# Reserving external IP addresses
########################################
gcloud compute addresses create lb-ipv4-1 \
  --ip-version=IPV4 \
  --global

#gcloud compute addresses create lb-ipv6-1 \
#  --ip-version=IPV6 \
#  --global

########################################
# Configuring the load balancing resources
########################################
#=======================================
# health check
gcloud compute health-checks create https https-basic-check \
    --port 443

#=======================================
# backend service
gcloud compute backend-services create partner-backend-service \
    --global-health-checks \
    --protocol=HTTPS \
    --port-name=https \
    --health-checks=https-basic-check \
    --global

gcloud compute backend-services create ep-backend-service \
    --global-health-checks \
    --protocol=HTTPS \
    --port-name=https \
    --health-checks=https-basic-check \
    --global

#=======================================
# Add instance groups as backends to the backend services
gcloud compute backend-services add-backend partner-backend-service \
    --balancing-mode=UTILIZATION \
    --max-utilization=0.8 \
    --capacity-scaler=1 \
    --instance-group=partner-${REGION_01} \
    --instance-group-zone=${INSTANCE_GROUP_ZONE_01} \
    --global

gcloud compute backend-services add-backend partner-backend-service \
    --balancing-mode=UTILIZATION \
    --max-utilization=0.8 \
    --capacity-scaler=1 \
    --instance-group=partner-${REGION_02} \
    --instance-group-zone=${INSTANCE_GROUP_ZONE_02} \
    --global

gcloud compute backend-services add-backend ep-backend-service \
    --balancing-mode=UTILIZATION \
    --max-utilization=0.8 \
    --capacity-scaler=1 \
    --instance-group=ep-${REGION_01} \
    --instance-group-zone=${INSTANCE_GROUP_ZONE_01} \
    --global

gcloud compute backend-services add-backend ep-backend-service \
    --balancing-mode=UTILIZATION \
    --max-utilization=0.8 \
    --capacity-scaler=1 \
    --instance-group=ep-${REGION_02} \
    --instance-group-zone=${INSTANCE_GROUP_ZONE_02} \
    --global

#=======================================
# URL map to route the incoming requests to the appropriate backend services
gcloud compute url-maps create web-map \
    --default-service ep-backend-service


#=======================================
# Google-managed SSL certificate
gcloud beta compute ssl-certificates create ep-ssl-cert \
  --domains ${FQDN}

#=======================================
# Create a target HTTPS proxy to route requests to your URL map
gcloud compute target-https-proxies create https-lb-proxy \
    --url-map web-map --ssl-certificates ep-ssl-cert

#=======================================
# Create two global forwarding rules to route incoming requests to the proxy
gcloud compute forwarding-rules create https-content-rule \
    --address=lb-ipv4-1\
    --global \
    --target-https-proxy=https-lb-proxy \
    --ports=443


########################################
# Sending traffic to your instances
########################################
gcloud compute addresses describe lb-ipv4-1 \
--format="get(address)" \
--global



set +x
echo "***********************************************"
echo "***********************************************"
echo "**         Create DNS records                **"
echo "***********************************************"
echo "***********************************************"
echo "--------------------------------------------------------------"
echo "Record name		Type	Value"
echo "--------------------------------------------------------------"
echo -ne "$FQDN	A	"
gcloud compute addresses describe lb-ipv4-1 --format="get(address)" --global
echo "$FQDN	CAA	0 issue \"pki.goog\""
echo "--------------------------------------------------------------"

echo "***********************************************"
echo "***********************************************"
echo "** and then run                              **"

#=========================================
# Confirm that your certificate resource's status is ACTIVE
#=========================================
# DO IT when DNS is ready
#=========================================
echo "** gcloud beta compute ssl-certificates list **"
echo "***********************************************"
echo "***********************************************"

