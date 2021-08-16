# Region for the AWS server.
variable "aws_region" { default = "eu-north-1" }
variable "aws_av_zone" { default = "eu-north-1a" }

# SSH key name
variable "key_name" { default = "%KEYNAME%" }

# Access Key ID from AWS.
variable "access_key" { default = "" }

# Secret Access Key from AWS
variable "secret_key" { default = "" }

# Unboundsecurity Entrypoint Server Name
variable "ep_host" { default = "%UNBOUND_EP%" }

# Unbound ekm client rpm file name
variable "ekm_client_rpm" { default = "%RPMFILE%" }

# Unboundsecurity client activation code
variable "activation_code" { default = "123456789" }

# Unboundsecurity partition name
variable "partition" { default = "%PARTITION%" }

# Unboundsecurtity key name
variable "rsa_key_name" { default = "%KEYNAME%" }

# wget ekm-client command
variable "wget_ekm_client" { default = "wget --no-verbose --no-check-certificate https://XXXXXXX/XXXXXXX.rpm --user=XXX --password='XXXX' -O /tmp/ekm-client.rpm" }

# Generate enceypted keyphase used by encfs
# 1. Extract public key from Unbound UKC
# ucl export -n aaa --output cert.pub
# 2. Encrypted keyphase using the public key
# head -c 128 /dev/random | openssl rsautl -encrypt -pubin -inkey ./cert.pub -oaep | base64
variable "enc_keyphase" { default = "%BASE64_ENC_KEY_PHASE"}
