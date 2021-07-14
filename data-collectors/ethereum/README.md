# Ethereum Data Collector

This sample provides a data collector for checking Ethereum. For general information about data collectors, 

For more information, including how to run the data collector, see the [User Guide](https://www.unboundsecurity.com/docs/CASP/CASP_User_Guide/Content/Products/CASP/CASP_User_Guide/Web_Interface.htm#Data).


The Ethereum Data Collector collects these attributes:
1. *contractAddress*
    - String attribute
    - for ETH asset has value - “ETH”
    - for ERC20 BOKKY - “ERC20-0x583CBBB8A8443B38ABCC0C956BECE47340EA1367”
1. *recipientAddress* - String attribute for destination address
1. *totalAmount* - Numeric attribute total amount for sign request 

## Prerequisites

- Java version 1.8 or newer
- Maven 3.5.4 or newer

## Build Instructions

Use Maven to build the data collector sample jar, with the following commands:

1. Install the data collector SDK jar file in your local Maven repository:
    
    `mvn install:install-file -Dfile=../jar/dc-sdk-<version>.jar -DgroupId=casp -DartifactId=dc-sdk -Dversion=1.0 -Dpackaging=jar`
2. Build the BOT:
    
    `mvn clean compile assembly:single`
	
 These commands create *eth-data-collector-compiled-sample-1.0-SNAPSHOT-jar-with-dependencies.jar* in the *target* folder.


## Data Collector Setup

The specified name and type are mandatory as the bot looks to supply a attribute template with this name and type.

1. Create an attribute template with name *contractAddress* and type *string*.
    - Set minimum to 1
    - Set maximum to 100
1. Create aa attribute template with name *recipientAddress* and type *string*.
    - Set minimum to 1
    - Set maximum to 100
1. Create a attribute template with name totalAmount and type numeric
    - Set minimum to 0
1. Create an attribute template group *ethTxGroup*. The suggested name is not mandatory.
1. Create a data collector in CASP with *ethTxGroup* as the attribute template group.
1. Activate the data collector. See the [User Guide section on Bots](https://www.unboundsecurity.com/docs/CASP/CASP_User_Guide/Content/Products/CASP/CASP_User_Guide/CASP_Bot.htm) for more information.
1. Run the data collector with all the required parameters.

## Policy Vault Setup

Create a [risk-based policy vault](https://www.unboundsecurity.com/docs/CASP/CASP_User_Guide/Content/Products/CASP/CASP_User_Guide/Web_Interface.htm#advanced-vaults).

*Note that the addresses must be uppercase and without 0x prefix.*

1. Use the vault attributes:
    - contractAddress -  (0x583cbBb8a8443B38aBcC0c956beCe47340ea1367 → “ERC20-0x583CBBB8A8443B38ABCC0C956BECE47340EA1367”)
    - recipientAddress - (0xCD258e7c51F8B29aE5A8b31f6D908AfFcBf1647F → CD258E7C51F8B29AE5A8B31F6D908AFFCBF1647F)
    - totalAmount - define in wei coin where 1 ether = 1,000,000,000,000,000,000 wei (1018), 0.09 eth = 90000000000000000 wei.
2. Create the vault policy with attribute rules based on contractAddress, recipientAddress, and totalAmount attribute templates.

## Run Ethereum Data Collector

Use the following command to start the data collector:

```
sudo java -jar casp-eth-data-collector-sample.jar -k -i eth_data_collector_id -u https://localhost/ -w 12345678
```
