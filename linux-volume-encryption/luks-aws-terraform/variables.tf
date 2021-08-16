# Region for the AWS server.
variable "aws_region" { default = "eu-north-1" }
variable "aws_av_zone" { default = "eu-north-1a" }

# SSH key name
variable "key_name" { default = "%KEYNAME%" }

# Access Key ID from AWS.
variable "access_key" { default = "" }

# Secret Access Key from AWS
variable "secret_key" { default = "" }

# Unboundsecurity Entrypoint Server encryption key url
variable "ep_url" { default = "https://%UNBOUND_EP%/api/v1/keys/%KEYNAME%/value" }

# Unboundserver user credentials
variable "ep_user_pwd" { default = "%USERNAME%@%PARTITION%:%PASSWORD%" }
