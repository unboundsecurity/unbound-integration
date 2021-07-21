# CipherTrace Data Collector Sample

This sample code implements a CASP Data Collector that returns a risk score computed by CiperTrace. [CipherTrace](https://ciphertrace.com/) is a risk intelligence service used for financial crime prevention and anti-money laundering compliance. It produces a risk score based on target address passed to it from the data collector.

**CASP data collectors** are independent components that calculate policy related attribute templates for transaction signing. Each data collector is associated with an attribute template group that contains the attribute templates.

Unlike participants, which can be human and require no development, data collectors by definition require development by the customer. See the [CASP Java SDK](https://www.unboundsecurity.com/docs/CASP/CASP_Developers_Guide/Content/Products/CASP/CASP_Participant_SDK/CASP_Java_SDK.htm) in the UKC Developers Guide for more information on creating the data collector client.

## Prerequisites
The following components are required:
- Maven 3 or newer 
- Java 1.8 or newer
- CipherTrace API key (used when starting the CASP data collector)

## Building and running
Use the following procedure to install the jar files to local Maven repository.
1. Download this repository.
2. Navigate to `unbound-integration/data-collectors/java-sdk`
3. Run the following Maven commands.
    ```
    mvn install:install-file -Dfile=client-sdk-1.0.jar -DgroupId=com.unboundtech.casp -DartifactId=client-sdk -Dversion=1.0-SNAPSHOT -Dpackaging=jar -DgeneratePom=true
    mvn install:install-file -Dfile=java-utils-1.0.jar -DgroupId=com.unboundtech.casp -DartifactId=java-utils -Dversion=1.0-SNAPSHOT -Dpackaging=jar -DgeneratePom=true
    mvn install:install-file -Dfile=samples-base-1.0.jar -DgroupId=com.unboundtech.casp -DartifactId=samples-base -Dversion=1.0-SNAPSHOT -Dpackaging=jar -DgeneratePom=true
    mvn install:install-file -Dfile=transaction-handlers-1.0.jar -DgroupId=com.unboundtech.casp -DartifactId=transaction-handlers -Dversion=1.0-SNAPSHOT -Dpackaging=jar -DgeneratePom=true
    ```
3. Navigate to `unbound-integration/data-collectors/ciphertrace`
4. Activate the data collector:
    ```
    mvn compile exec:java -Dexec.mainClass=com.unboundtech.casp.datacollector.cyphertrace.sample.Main "-Dexec.args=-u <CASP_URL> -i <DATA_COLLECTOR_ID> -w <KEY_STORE_PWD> -c <ACTIVATION_CODE>"
    ```
5. Run the data collector:
    ```
    mvn compile exec:java -Dexec.mainClass=com.unboundtech.casp.datacollector.cyphertrace.sample.Main "-Dexec.args=-u <CASP_URL> -i <DATA_COLLECTOR_ID> -w <KEY_STORE_PWD> -a <CIPHERTRACE_API_KEY>"
    ```
