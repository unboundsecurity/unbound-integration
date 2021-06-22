# Unbound Key Control for Tessera

## What is UKC?

Unbound has decoupled trust from infrastructure. Based on cryptographic
breakthroughs that draw strength from math (not matter), Unbound Key Control
("UKC") is the first solution to offer a truly abstracted key management that meets the
high levels of security once only attainable through hardware-based perimeter-centric
models. Built upon Unbound’s platform-agnostic Distributed Trust Platform, UKC can
be deployed across your entire decentralized hybrid cloud and geo-distributed
environments without disrupting existing application workflows. All key management
and user management operations are fully automated using a REST API, giving you the
ability to scale up or down, create partitions and users, register clients and revoke keys
immediately across your entire global infrastructure. UKC enables you to stretch the
boundaries of your security infrastructure to centrally manage all cryptographic keys,
secrets and certificates across your network (including private cloud, public cloud, and
virtual machines).

Unbound Key Control ("UKC") protects secrets such as cryptographic keys by ensuring they never exist in complete form.

## What is Tessera?

[Tessera](https://github.com/jpmorganchase/tessera) is a stateless Java system that is used to enable the encryption, decryption, and distribution of private transactions for [Quorum](https://github.com/jpmorganchase/quorum/).

# Using UKC with Tessera

<a name="prerequisites"></a>
## Prerequisites
- Install UKC (EP, Partner, and Auxiliary servers) version 2.0.2001 and up.
- Using the EP server, create the  UKC partition that will store keys used by the Tessera server (referred below as the PARTITION_NAME).
- Tessera *jar* file - this integration was tested with *tessera-app-0.10.6-app.jar*.


## Preparing Tessera Server

To allow using the UKC cryptographic services, the UKC SSL client certificate and its SSL trust CA certificate must be installed on the Tessera server, and the environment variables used by the UKC software must be set accordingly.

The UKC certificates may be obtained using one of the following methods:

1. Explicitly create the certificate on the EP server - [Create and Download the Certificate](#FullCert).
1. Obtain an ephemeral certificate from the server - [Obtain Ephemeral Certificate](#Ephemeral).
1. Install UKC client software on the Tessera server and use the standard UKC client registration procedure to obtain the  certificates - [Install with a UKC Client](#Withclient).

<a name="FullCert"></a>
### Option 1: Create and Download the Certificate
1. To create the certificate, run the following command on the EP server.

    ```
    ucl client create --mode FULL --partition <PARTITION_NAME> --password <PARTITION_PASSWORD> --name <TESSERA_HOST_NAME> --output ./tessera_client.pfx --pfx_password <PFX_PASSWORD>
    ```
    
   Notes:
   - Use the <PARTITION_NAME> assigned in [Prerequisites](#prerequisites).
   - Specify the SO password that allows accessing the partition in the <PARTITION_PASSWORD>.
   - Specify Tessara's hostname in the <TESSERA_HOST_NAME>. This value will be included in the certificate.
   - In the --output option, specify the name of the certificate file, for example, "tessera-client.pfx".
   - Set the password that protects the content of the certificate in the <PFX_PASSWORD>.
       

   By default, this certificate is valid for three years. To change the default, append the following option:
    
    ```
    --cert_validity <Validity period in minutes>
    ```
    
   
1. To obtain the UKC SSL trust certificate (ukc_ca.p7b) run the following command on the EP server.
   
   ```
    ucl root_ca --output ./ukc_ca.p7b
    ```   

1. Upload these two files to the Tessera server.


1. Configure Environment Variables on Tessera server

    The following environment variables need to be configured:

    ```
    export UKC_SERVERS=<EP_HOSTNAME>
    export UKC_PARTITION_NAME=<PARTITION_NAME>
    export DYLOG_ENABLED=1
    export UKC_CA=<path-to-ukc_ca.p7b>
    export UKC_PFX=<path-to-tessera_client.pfx>
    export UKC_PFX_PASS=<PFX_PASSWORD>
    ```

<a name="Ephemeral"></a>
### Option 2: Obtain Ephemeral Certificate
This option has the following advantages

- On the EP server, you create a template that is used to derive certificates. You specify for how long this template is valid and its access credentials. As long as you know the credentials (name and access code) you can use it  many times without the further need to manage the EP server.

- You obtain the certificate for the specific period in the granularity of minutes. Once this period expires you may obtain the certificate for another period and so forth based on your requirements. For example, you can obtain the certificate on demand for the fixed period, or schedule its availability in advance.

The control is totally on your side without any further engagement with the UKC server admin.


1. To create the certificate template, run the following command on the EP server.

    ```
    ucl  client create --mode template --name <TEMPLATE_NAME> --partition <PARTITION_NAME>
    ```
    Notes:
    - Use the <PARTITION_NAME> assigned in [Prerequisites](#prerequisites).
    - Specify the SO password that allows accessing  the partition in  the <PARTITION_PASSWORD>

    By default, this template is valid for 30 minutes and the certificates derived from it are valid for 20 minutes. To change the defaults, add the following options:
    
    ```
    --ac_validity <The template validity period in minutes>
    --cert_validity <Validity period of each derived certificate>
    ```
    
    The output of this command is <ACTIVATION_CODE>. Together with the <TEMPLATE_NAME> they will let Tessera to obtain its SSL client certificate (refer to [UKC User Guide](https://www.unboundsecurity.com/docs/UKC/UKC_User_Guide/HTML/Content/Products/UKC-EKM/UKC_User_Guide/PartitionBindings/EnrollAC.html#h3_4)).
    
 1. To obtain the UKC SSL trust certificate (ukc_ca.p7b) run the following command on the EP server.
   
    ```
    ucl root_ca --output ./ukc_ca.p7b
    ```   

1. Upload this file to the Tessera server.

    
1. Configure Environment Variables on Tessera server

    The following environment variables need to be configured:

    ```
    export UKC_SERVERS=<EP_HOSTNAME>
    export UKC_PARTITION_NAME=<PARTITION_NAME>
    export DYLOG_ENABLED=1
    export UKC_CA=<path-to-ukc_ca.p7b>
    export UKC_TEMPLATE_NAME=<TEMPLATE_NAME>
    export UKC_ACTIVATION_CODE=<ACTIVATION_CODE>

    ```
<a name="Withclient"></a>
### Option 3: Use the UKC Client Software

If you installed the UKC Client Software on the Tessera server (refer to [UKC User Guide](https://www.unboundsecurity.com/docs/UKC/UKC_User_Guide/HTML/Content/Products/UKC-EKM/UKC_User_Guide/Installation/ClientInstallation.html)), you can choose the following standard UKC client creation and registration approach to implicitly obtain the required certificates.


1. To create the certificate template, run the following command on the EP server.

    ```
    ucl client create --mode activate --name <CLIENT_NAME> --partition <PARTITION_NAME> --password <UKC_PASSWORD>
    ```
1. On the Teserra server. Edit the configuration file on the UKC client, found in:

    `/etc/ekm/client.conf`
    
    Update the server name of the UKC EP. For example:
    
    `servers=<EP_HOSTNAME>`
1. On the Teserra server. Obtain the necessary certificates
    ```
    ucl register --code <ACTIVATION_CODE> --name <CLIENT_NAME> --partition <PARTITION_NAME>
    ```
## Create the Unbound Encryption jar File
Create the jar file for Unbound encryption, which is used in the next section.

1. Get this repo.

    `git clone https://github.com/unboundsecurity/unbound-integration.git`
1. Access the local directory.

    `cd unbound-integration/tessera`
1. Use Maven to create the package.

    `mvn package`
1. Access the target directory.

    `cd target`
1. The output file has a format similar to:

    `encryption-ub-0.10.5-all.jar`

## Using UKC for Encryption with Tessera

1. Create the key and public key. The *tessera-app.jar* file is one of the prerequisites. The *encryption-ub.jar* file was created in the previous section.
    ```
    java -cp <tessera-app>.jar:<encryption-ub>.jar com.quorum.tessera.launcher.Main -keygen  --encryptor.type CUSTOM -filename <KEY-FILENAME>
    ```
    This command creates 2 files:
    - `<KEY-FILENAME>.key`
    - `<KEY-FILENAME>.pub`
		
2. Create the Tessera configuration file.

    Create a file containing the Tessera configuration information. See [here](https://github.com/jpmorganchase/quorum-examples#experimenting-with-alternative-curves-in-tessera) for more information.

    Set the *encryptor* type to *CUSTOM*:
    ```
    "encryptor": {
        "type": "CUSTOM"
    },
    ```

    Set the paths to the keys:
    ```
	"keys": {
        "passwords": [],
        "keyData": [
            {
                "config": "$(cat $<FILENAME>.key)",
                "publicKey": "$(cat $<FILENAME>.pub)"
            }
        ]
    },	
   ```
