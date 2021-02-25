# Integrating UKC with a CA
Unbound Key Control ("UKC") can be integrated with a Certificate Authority ("CA"). Intructions for integration with various CA's are provided in the following sections.

## DigiCert Integration

This example demonstrates an automated integration of UKC with DigiCert. The full cycle is provided, starting from CSR creation and ending with a signed certificate import to the Unbound server. 

The SSL certificate is obtaind from DigiCert via the REST API.


### Prerequisites
You need the following package for this integration:
- [Unbound PKCS11 Python package](https://github.com/unbound-tech/unbound-pypkcs11 "Unbound PKCS#11 Python package")

### Documentation
The following links provide information that may be helpful for this integation:
- [UnboundTech Python interface to PKCS#11](http://unbound-pypkcs11.readthedocs.io/en/latest/ "UnboundTech Python interface to PKCS#11")
- [DigiCert Web Services API](https://www.digicert.com/services/v2/documentation "DigiCert Services API")
- [SSL Certificate Validation Process from DigiCert ](https://www.digicert.com/ssl-validation-process.htm "SSL Certificate Validation Process from DigiCert ")

### Flow Overview
To integrate UKC into DigiCert, use the following process:
1. Run '*digicert_place_order.py*' script to generate a Certificate Request and send it to DigiCert.
2. Get your order validated and approved by DigiCert.
3. Run '*digicert_download_cert.py*' script to download the certificate chain and import to the Unbound server.

### Detailed Flow 

#### Step 1: Run the Place Order Script
Run '*digicert_place_order.py*' script to generate a Certificate Request and send it to DigiCert. This script:
- Generates an RSA key pair (*c_generate_key_pair* function)
- Generates a CSR (*dyc_create_x509_request* function)
- Posts the order to DigiCert (*order/certificate/ssl_plus*)
- Parses the reply, outputs Order ID 

#### Step 2: Order Validation
Get your order validated and approved by DigiCert. In most cases, this is automatically processed. In other cases, such as the first time that you use the CA, you may receive an email with instructions or need to contact DigiCert directly.

#### Step 3: Run the Download Certificate Script
Run '*digicert_download_cert.py*' script to download the certificate chain and import to the Unbound server. This script:
- Receives Order ID as parameter
- Obtains the Certificate ID by Order ID (GET to *https://www.digicert.com/services/v2/order/certificate/* ; optional if you already have Certificate ID)
- GETs Certificates from DigiCert (GET to *https://www.digicert.com/services/v2/certificate/{certificate_id}/download/platform*)
- Unpacks the received ZIP archive
- Stores each certificate on the Unbound server (call to *c_create_object*)
