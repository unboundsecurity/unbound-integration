# Region for the AWS server.
variable "aws_region" { default = "eu-north-1" }
variable "aws_av_zone" { default = "eu-north-1a" }

# SSH key name
variable "ssh_key_name" { default = "%KEYNAME%" }

# Access Key ID from AWS.
variable "access_key" { default = "" }

# Secret Access Key from AWS
variable "secret_key" { default = "" }

# Unboundsecurity Entrypoint Server Name
variable "ep_host_name" { default = "%UNBOUND_EP%" }

# Unboundsecurity Entrypoint Server IP
variable "ep_host_ip" { default = "" }

# Unbound ekm client rpm file name
variable "ekm_client_rpm" { default = "%RPMFILE%" }

# Unboundsecurity client name
variable "client_name" { default = "client-name" }

# Unboundsecurity client activation code
variable "activation_code" { default = "123456789" }

# Unboundsecurity partition name
variable "partition" { default = "%PARTITION%" }

# Unboundsecurtity key name
variable "rsa_key_name" { default = "%KEYNAME%" }

# wget ekm-client command
variable "wget_ekm_client" { default = "wget --no-verbose --no-check-certificate https://unbound-ekm-client.s3.us-west-1.amazonaws.com/ekm-client-2.0.2010.38364.el8.x86_64.rpm -O /tmp/ekm-client.rpm" }

# Generate encrypted keyphase used by encfs
# METHOD 1
# head -c 128 /dev/random > /tmp/rnd.txt
# ucl encrypt -i /tmp/rnd.txt -o /tmp/enc.txt -p code-sign -n rsakey1
# rm -rf /tmp/rnd.txt
# base64 /tmp/enc.txt | tr -d "\n"
# METHOD 2
# 1. Extract public key from Unbound UKC
# ucl export -n aaa --output cert.pub
# 2. Encrypted keyphase using the public key
# head -c 128 /dev/random | openssl rsautl -encrypt -pubin -inkey ./cert.pub -oaep | base64
variable "enc_keyphase" { default = "%BASE64_ENC_KEY_PHASE"}
