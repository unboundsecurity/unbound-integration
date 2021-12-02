# CipherTrace Data Collector Sample

This sample code implements a CASP Data Collector that returns a risk score computed by CiperTrace. [CipherTrace](https://ciphertrace.com/) is a risk intelligence service used for financial crime prevention and anti-money laundering compliance. It produces a risk score based on target address passed to it from the data collector.

**CASP data collectors** are independent components that calculate policy related attribute templates for transaction signing. Each data collector is associated with an attribute template group that contains the attribute templates.

Unlike participants, which can be human and require no development, data collectors by definition require development by the customer. See the [CASP Java SDK](https://www.unboundsecurity.com/docs/CASP/CASP_Developers_Guide/Content/Products/CASP/CASP_Participant_SDK/CASP_Java_SDK.htm) in the UKC Developers Guide for more information on creating the data collector client.

## Prerequisites
The following components are required:
- Maven 3 or newer 
- Java 1.8 or newer
- CipherTrace API key (used when starting the CASP data collector)

## Build and Run Instructions

Use Maven to build the CipherTrace data collector sample jar, with the following commands:

1. Acquire CASP SDK jar from Unbound repo
 
2. Navigate to `unbound-integration/data-collectors/casp-sdk`

3. Install the casp-sdk.jar file in your local Maven repository
    ```
    mvn install:install-file -Dfile=casp-sdk-<version>.jar -DgroupId=com.unboundtech.casp -DartifactId=casp-sdk -Dversion=1.0 -Dpackaging=jar -DgeneratePom=true`
   
    ```
4. Navigate to `unbound-integration/data-collectors/ciphertrace`
    
5. Build *ciphertrace-data-collector-sample-1.0-SNAPSHOT-jar-with-dependencies.jar* in the *target* folder.
    ```
    mvn clean compile assembly:single
   
    ```

6. Activate the data collector:
    ```
    java -jar ./target/ciphertrace-data-collector-sample-1.0-SNAPSHOT-jar-with-dependencies.jar  -u <CASP_URL> -i <DATA_COLLECTOR_ID> -w <KEY_STORE_PWD> -c <ACTIVATION_CODE>
      
    ```
7. Run the data collector:
    ```
    java -jar ./target/ciphertrace-data-collector-sample-1.0-SNAPSHOT-jar-with-dependencies.jar  -u <CASP_URL> -i <DATA_COLLECTOR_ID> -w <KEY_STORE_PWD> -a <CIPHERTRACE_API_KEY>
   
    ```
