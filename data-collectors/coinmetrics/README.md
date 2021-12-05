# CoinMetrics Data Collector Sample

This sample code implements a CASP Data Collector that returns a BTC transaction volume in USD using CoinMetrics. [CoinMetrics](https://coinmetrics.io/) is a service that provides access to historical and real-time network and market data.

**CASP data collectors** are independent components that calculate policy related attribute templates for transaction signing. Each data collector is associated with an attribute template group that contains the attribute templates.

Unlike participants, which can be human and require no development, data collectors by definition require development by the customer. See the [CASP Java SDK](https://www.unboundsecurity.com/docs/CASP/CASP_Developers_Guide/Content/Products/CASP/CASP_Participant_SDK/CASP_Java_SDK.htm) in the UKC Developers Guide for more information on creating the data collector client.

The CoinMetrics Data Collector Sample retrieves BTC to USD using the [getTimeseriesAssetMetrics](https://docs.coinmetrics.io/api/v4#operation/getTimeseriesAssetMetrics) API.

Note the following about the return result:
1. The BTC to USD rate can be retrieved based only on day frequency.
2. The per day returns a BTC to USD rate at 00:00 UTC time, so today's request is returns the BTC to USD rate from yesterday.
3. The data collector calculates a BTC transaction volume in USD based on data provided by the [getTimeseriesAssetMetrics](https://docs.coinmetrics.io/api/v4#operation/getTimeseriesAssetMetrics) API rate.
4. **Important**: The data collector returns *transaction.value.in.dollars* rounding down to the nearest dollar.

See [here](https://docs.coinmetrics.io/api) for the CoinMetrics documentation.

## Prerequisites
The following components are required:
- Maven 3 or newer 
- Java 1.8 or newer
- CoinMetrics API key (used when starting the CASP data collector)

## Build and Run Instructions

Use Maven to build the CoinMetrics data collector sample jar, with the following commands:

1. Download the CASP SDK package from the Unbound repository (using the link provided to you by [Unbound Support](mailto:support@unboundsecurity.com)) and extract the JAR file. 
2. Copy the JAR file to `unbound-integration/data-collectors/casp-sdk`.
3. Install *casp-sdk-<version>.jar* in your local Maven repository.
    ```
    mvn install:install-file -Dfile=casp-sdk-<version>.jar -DgroupId=com.unboundtech.casp -DartifactId=casp-sdk -Dversion=1.0 -Dpackaging=jar -DgeneratePom=true`
    ```
4. Navigate to `unbound-integration/data-collectors/coinmetrics`.
5. Build *coinmetrics-data-collector-sample-1.0-SNAPSHOT-jar-with-dependencies.jar* in the *target* folder.
    ```
    mvn clean compile assembly:single
    ```
6. Activate the data collector:
    ```
    java -jar ./target/coinmetrics-data-collector-sample-1.0-SNAPSHOT-jar-with-dependencies.jar  -u <CASP_URL> -i <DATA_COLLECTOR_ID> -w <KEY_STORE_PWD> -c <ACTIVATION_CODE>
    ```
7. Run the data collector:
    ```
    java -jar ./target/coinmetrics-data-collector-sample-1.0-SNAPSHOT-jar-with-dependencies.jar  -u <CASP_URL> -i <DATA_COLLECTOR_ID> -w <KEY_STORE_PWD> -a <COINMETRICS_API_KEY>
    ```
