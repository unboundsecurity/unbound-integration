# Volume encryption with EncFS

## Overview
Protecting sensitive files with Unbound MPC technology is supported using [EncFS](https://en.wikipedia.org/wiki/EncFS).

EncFS provides transparent file encryption for an arbitrary volume and is supported by many platforms. The volume can be mounted to any media supported by the host including cloud volumes like [Amazom EBS](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/ebs-using-volumes.html).  

Files are encrypted using a volume key and a password is used to decrypt that key.

This guide provides instructions and scripts for using Unbound MPC technology to protect the volume encryption key password. 
This provides the following benefits:
* Strong password protection with MPC technology
* Key rotation for increased security 

## Prerequisites
### Server
A working [Unbound CORE Cluster](https://www.unboundsecurity.com/docs/TechDocs/Unbound_Doc_Versions-HTML/Content/Products/UnboundDocLibrary/Technical_Document_Versions.htm#UKC) server is required.  
You'll need to use the server URL / network name in the *Configuration* section below.  

The following objects should be created:
#### Encryption Key
[Create an RSA key](https://www.unboundsecurity.com/docs/UKC/UKC_Interfaces/Content/Products/UKC-EKM/UKC_User_Guide/UG-If/uiSO/KeyTab.html#Create2) with permissions for `Decrypt`.  
You'll need to use the key name and partition name as described in the *Configuration* section below
#### Ephemeral Client
If your'e going to use Unbound CORE Client for communications you'll need to prepare an [Ephemeral client template](https://www.unboundsecurity.com/docs/UKC/UKC_Interfaces/Content/Products/UKC-EKM/UKC_User_Guide/UG-If/uiSO/ClientsTab.html#Multi-us).  
You'll need to use the *Client name* and *Activation code* in the *Configuration* section below.
#### Partition User
To be able to decrypt with the key, you'll need to have a partition user with appropriate role with permission for `Decrypt` operation with the encryption key. The built-in `user` role might be used for this purpose.  
When using the REST API communication method, the user **must have a password set**.
### Client
The client machine pre-requisites:
1. Linux with `bash`  
   The scripts in this repo are targeted for a **Linux** machine. Although the overall solution can be migrated to any other OS which is supported by EncFS.
1. `encfs` package
2. Copy the `bash-scripts` folder from this repository into the client machine and _add it to system PATH_  
   To check if this step was successfull try to run `ub-init` from a bash shell and verifies it is found.
4. Environment variables should be set as described in the *Configuration* section below according to the chosen communication method.

#### Communications with Unbound CORE server can be done in one of two ways:
   1. Using [CORE REST API](http://www.unboundsecurity.com/docs/ukc_rest/ukc.html)  
      `curl` is required for this option to work.  
      This option is easier to deploy but may be less secure when used without client TLS certificate.
      This option requires that you create a partition user with a password.
   1. Using Unbound CORE Client  
      This requires [installation of Unbound CORE Client](https://www.unboundsecurity.com/docs/UKC/UKC_Installation/Content/Products/UKC-EKM/UKC_User_Guide/UG-Inst/ClientInstallation.html) package.  
      This option is more secure since it uses Unbound proprietry trust algorithms including mutual TLS.  
   The `ub-init` scripts can be used to test which of the communication options is used and configure it

## Configuration
The `ub-get-password` script uses the following environment variables
### Common settings
* `UB_PARTITION` (Required, Defaults to `encfs`)  
  The name of the Unbound CORE partition containing the encryption key.
* `UB_KEY_NAME` (Required)  
  The name of the key to use for encryption. Must be an RSA key in UB_PARTITION, with Decrypt operation permission.
* `UB_USER` (Required, Defaults to `user`)  
  The partition user name. Must be a user with permissions for Decrypt with the encryption key. 
* `UB_PASSWORD_FILE` (Required, Default is `encfs_pass.txt` in current folder)  
  The path for saving the encrypted password data.
### REST API settings
These settings are only required if your'e using the REST API communication method.
* `UB_USER_PASSWORD`  
  The partition user password.  
* `UB_CORE_URL`  
  The URL of the Unbound CORE server. For example: `https://unbound-server.com`
### CORE Client settings
These settings are only required if you're using Unbound CORE Client communication method and you are using the `ub-init` script for configuring it.  
If you already configured your'e client manually you don't need these.
* `UB_SERVER_NAME` (Required only when using CORE Client)  
  The network name of the CORE Server
* `UB_CLIENT_TEMPLATE_NAME`  
  The name of the Ephemeral client template to use for communications with the server.  
* `UB_CLIENT_ACTIVATION_CODE`  
  The client secret activation code initiating trusted communications.
### Checking the configuration
To check if the configuration is correct:
1. Run `ub-init` and make sure you see a message with the server version.
2. Run `ub-get-password` and verify you get a new password printed to the screen.

## Starting EncFS with Unbound CORE
To use EncFS with Unbound CORE, start EncFS Daemon with the password encrypted with Unbound CORE 
```
encfs --extpass="ub-get-password" --standard /your-encrypted-data-folder /your-mounted-data-folder
```
## Key rotation
This solution automatically supports key-rotation.  
`ub-get-password` will always encrypt using the latest version of the key.

## Demo
### Demo script
To see an interactive demo after the client is configured, you can run `ub-start-encfs-demo`  
### Demo with Docker
To run the demo on docker you'll need to have [Docker installed](https://docs.docker.com/get-docker/) on your system.
1. Open a bash shell in the `docker` subfolder of this repository.
2. (Optional), if you want to use Unbound CORE Client, edit the Dockerfile and uncomment the relevant client installation command. 
   You must have permissions to download Unbound CORE Client installation
3. Edit the `docker-compose` file and set the relevant configuartion parameters.
4. Run `docker-compose build && docker-compose run ukc-client`
5. Follow the on-screen instructions

