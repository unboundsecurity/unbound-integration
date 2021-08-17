# Using CORE with AWS Lambda

[AWS Lambda](https://aws.amazon.com/lambda/) is a serverless compute service that lets you run code without provisioning or managing servers, creating workload-aware cluster scaling logic, maintaining event integrations, or managing runtimes. With Lambda, you can run code for virtually any type of application or backend service - all with zero administration. AWS Lambda lets you use Unbound CORE for specific tasks without the need of installing and maintaining any devices, virtual machines, or Dockers.

To achieve this integration, we use Unbound CORE's *clientless* mode for the Unbound Java JCE Provider. The clientless mode is not dependent on any native library and can be used in a pure Java environment.

## Configuration

The following instructions describe how to develop an AWS Lamda function that works with CORE and the Unbound Java Provider. 

**As an example, it demostrates tokenization and detokenization of an email address.**

## Prerequisites

The following are required for this integration:
- Unbound CORE 2.0.2010 or later, installed and running with an accessible partition.
- A template client ready for registering new clients.
- Unbound CORE Root CA certificate.
- A PRF key (used for tokenization). You can generate this key with the command:
    ```
    ucl generate -t prf -p <PART> -n <KEY_NAME>
    ```
- Maven (latest)
    

## Download and Build

1. Download the code from the GitHub repo, found [here](https://github.com/unboundsecurity/unbound-integration/tree/master/saas-providers/aws-lambda).
2. Uncompress the zip archive.
3. Run the following command to build the code:
    `mvn clean package shade:shade`

## Create the AWS Lambda Function

Create 2 functions in AWS Lambda, as described [here](https://docs.aws.amazon.com/lambda/latest/dg/getting-started.html). One function is for tokenization and one function is for detokenization.

Notes: 
1. This example was tested using Java 8 on Amazon Linux 1.
2. Set the handler methods to these names:
    - com.unbound.TokenizeHandler
    - com.unbound.DeTokenizeHandler

## Environment Variables 

The CORE clientless provider depends on a set of environment variables that defines the needed properties to connect to CORE. These variables are set in the AWS Lambda interface.

- UKC_SERVERS - the hostname of the Entry Point
- UKC_PARTITION_NAME - the CORE partition
- UKC_NO_NATIVE - defines the provider as clientless, and must be set to 1.
- UKC_CA_DATA - the CORE Root CA. It is the *.p7b* content in HEX format. 
    For example:
	```
	ucl root_ca -o ca.p7b
    od -t x1 -An ca.p7b |tr -d '\n '
    ```
- UKC_TEMPLATE_NAME - the template client name
- UKC_ACTIVATION_CODE - activation code used for registration

Here is an example of the environment variables:
```
UKC_ACTIVATION_CODE: '3073800115123577'
UKC_CA_DATA: >-
  308201e106092a864886f70d010702a08201d2308201ce0201013100300a06082a864886f70d0107a08201b7308201b33082015aa003020102020904b9d39425f4ff8499300a06082a8648ce3d0403023021311f301d06035504031316556e626f756e6420554b4320526f6f74204341204731301e170d3231303732373136353835335a170d3239303732373136353835335a3021311f301d06035504031316556e626f756e6420554b4320526f6f742043412047313059301306072a8648ce3d020106082a8648ce3d0301070342000495ab783d64363b14f27e9b72fcd057b80de8b5e12c27322509a4777f676df09b61db06c5e06151352aa771b1716c0e2c481086066f30c9a2e0f439da3d989c46a37b3079302b0603551d230424302280204d1e9277223214149600ba13c8b0a26c1eaaf62fa8d7bde01a6a65dd1a95b9ea300f0603551d130101ff040530030101ff300e0603551d0f0101ff0404030201c630290603551d0e042204204d1e9277223214149600ba13c8b0a26c1eaaf62fa8d7bde01a6a65dd1a95b9ea300a06082a8648ce3d0403020347003044022052a12efd176d8e923ca2c48175634157c0f25418257cd8c3c1439ddf7b5b5d0702206f57e31781ead9196b02da69b1b02e716529e6661218f5d2d07f8519c8d6ec473100
UKC_NO_NATIVE: '1'
UKC_PARTITION_NAME: part1
UKC_SERVERS: ep.com
UKC_TEMPLATE_NAME: template
```


## Testing

In the AWS Lambda console you can test the function by sending it a JSON string.

**Tokenization**

Send the following JSON to the tokenization function:
```
{
  "keyId": "<KEY_NAME>",
  "value": "john.doe@email.com",
  "tweak": "TWEAK",
  "maxSize" : 100
}
```

Sample result:

```
{
  "uid": "<KEY_NAME>",
  "value": "aiQk48iSuqZzLre@CP8DB8q9pjaisFGFhk3sL30JRDimtbQriDHR715CnuiJC6hcqP72U8GbmcGxmP40hQfHcwcCyRAGNtpf.mo",
  "tweak": "TWEAK"
}
```


**Detokenization**

Send the following JSON to the tokenization function:
```
{
  "keyId": "<KEY_NAME>",
  "value": "aiQk48iSuqZzLre@CP8DB8q9pjaisFGFhk3sL30JRDimtbQriDHR715CnuiJC6hcqP72U8GbmcGxmP40hQfHcwcCyRAGNtpf.mo",
  "tweak": "TWEAK"
}
```

Sample result:

```
{
  "uid": "<KEY_NAME>",
  "value": "john.doe@email.com",
  "tweak": "TWEAK"
}
```

