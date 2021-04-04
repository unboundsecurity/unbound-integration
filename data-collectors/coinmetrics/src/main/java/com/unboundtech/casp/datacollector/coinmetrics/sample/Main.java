package com.unboundtech.casp.datacollector.coinmetrics.sample;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.unboundtech.casp.desktop.bot.KeyStoreStorage;
import com.unboundtech.casp.desktop.dc.DataCollectorSdk;
import com.unboundtech.casp.desktop.dc.DataCollectorSdkInitBuilder;
import com.unboundtech.casp.desktop.signer.CaspLog;
import com.unboundtech.casp.desktop.signer.CaspStatus;
import com.unboundtech.casp.desktop.signer.Log4jLogger;
import com.unboundtech.casp.desktop.signer.network.JavaRestClient;
import com.unboundtech.casp.service.txhandlers.*;
import com.unboundtech.casp.service.txhandlers.errors.BadTransactionException;
import com.unboundtech.utils.Utils;
import org.apache.commons.cli.*;

import java.math.BigDecimal;
import java.math.BigInteger;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.util.*;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

public class Main {
    private static final ScheduledExecutorService poolExecutor = new ScheduledThreadPoolExecutor(1);
    public static CoinMetricsQueryService service;
    public static BitcoinTransactionHandler btcHandler = new BitcoinMainNetTransactionHandler();


    public static void main(String[] args) throws Exception {
        CommandLineParser parser = new DefaultParser();
        Options options = new Options();

        options.addOption("i", "dataCollectorId", true, "Data collector ID");
        options.addOption("c", "activation-code", true, "The activation code provided by the CASP server");
        options.addOption("w", "keystorepass", true, "Keystore password");
        options.addOption("v", "verbose", false, "Enable verbose logging");
        options.addOption("u", "server-url", true, "CASP server url");
        options.addOption("k", "insecure", false, "Allow connections without certificate verification");

        options.addOption("a", "coinmetrics-api-token", true, "CoinMetrics API key");

        HelpFormatter formatter = new HelpFormatter();
        CaspLog.turnOnLogging();
        CaspLog.setLogLevel(CaspLog.WARN);
        CaspLog.setLogger(new Log4jLogger());

        CommandLine line = parser.parse(options, args);
        if (line.hasOption("v")) {
            CaspLog.setLogLevel(CaspLog.VERBOSE);
        }

        if (!line.hasOption("i")) {
            System.err.println("Data Collector ID is missing");
            formatter.printHelp(" ", options);
            return;
        }

        String keyStorePassword = getKeyStorePassword(line);
        if ((keyStorePassword != null) && (keyStorePassword.length() > 0)) {
            System.out.println("Keystore password was provided successfully");
        }else{
            System.out.println("Keystore password was not provided");
            return;
        }

        if (!line.hasOption("u")) {
            System.err.println("CASP Server URL is missing");
            formatter.printHelp(" ", options);
            return;
        }

        String dcId = line.getOptionValue("i");

        System.out.println("Starting CASP Data Collector");
        System.out.println("Data collector version: " +  DataCollectorSdk.getInstance().sdkVersion());
        System.out.println("Data collector ID: " +  dcId);

        boolean allowInsecureConnection = line.hasOption("k");

        KeyStoreStorage keyStoreStorage = new KeyStoreStorage(dcId, keyStorePassword);
        JavaRestClient javaRestClient = new JavaRestClient(line.getOptionValue("u"), allowInsecureConnection, keyStoreStorage, keyStorePassword);

        String serverUrl = line.getOptionValue("u");
        System.out.println("Server URL: " +  serverUrl);

        CaspStatus initStatus = new DataCollectorSdkInitBuilder(keyStoreStorage, keyStoreStorage, javaRestClient)
                .init();

        if (initStatus.getCode() != 0) {
            System.out.println(String.format("Init failed. exiting. code: %d msg: %s", initStatus.getCode(), initStatus.getDescription()));
            return;
        } else {
            System.out.println("init was successful");
        }

        if (line.hasOption("c")) {
            CountDownLatch latch = new CountDownLatch(1);
            DataCollectorSdk.getInstance().activateDataCollector(dcId, line.getOptionValue("c"), status -> {
                if (status.getCode() == 0) {
                    System.out.println("activation successful");
                } else {
                    System.err.println("DC activation failed. " +  status.getDescription());
                }
                latch.countDown();
            });
            latch.await();
            return;
        }

        if (!line.hasOption("a")) {
            System.err.println("CipherTrace API key is missing");
            formatter.printHelp(" ", options);
            return;
        }

        System.out.println("Starting data collection");

        service = new CoinMetricsQueryService(line.getOptionValue("a"));
        poolExecutor.scheduleAtFixedRate(Main::calcBTCTransactionVolumeInUSD, 0, 10, TimeUnit.SECONDS);
        poolExecutor.awaitTermination(9999, TimeUnit.DAYS);
    }

    private static String getKeyStorePassword(CommandLine line) {
        if (line.hasOption("w")) {
            return line.getOptionValue("w");
        } else {
            return new String(System.console().readPassword("%s :  ", "Please, enter keystore password"));
        }
    }


    public static void calcBTCTransactionVolumeInUSD(){
        DataCollectorSdk.getInstance().getDataCollectionRequest((getDataCollectionRequestStatus, dataCollectionRequest) -> {
            if (getDataCollectionRequestStatus.getCode() == CaspStatus.DY_ENO_ENTITY) {
                System.out.println(getDataCollectionRequestStatus.getDescription());
                return;
            }

            if (getDataCollectionRequestStatus.getCode() != 0) {
                System.err.println("failed to retrieve data collection request. " +  getDataCollectionRequestStatus.getDescription());
                return;
            }
            System.out.println("data collection details: " +  dataCollectionRequest.toString());

            SignRequest signRequest = Utils.fromJson(dataCollectionRequest.getSignRequest(), SignRequest.class);
            if (signRequest.rawTransactions.size() == 0) {
                System.err.println("'rawTransactions' are missing from sign request");
                return;
            }

            Optional<TransactionHandler> handler = getTransactionHandler(signRequest);

            if(handler.isPresent()){
                List<DetailedTransaction> detailedTransactions;
                try {
                    detailedTransactions = handler.get().decode(signRequest.dataToSign, signRequest.rawTransactions, signRequest.publicKeys, new HashSet<>(signRequest.publicKeys));
                } catch (BadTransactionException e) {
                    System.err.println("failed to decode BTC transaction");
                    return;
                }

                BigInteger txVolumeInSatoshi = detailedTransactions.stream()
                        .map(detailedTransaction -> detailedTransaction.getAmount())
                        .reduce(new BigInteger(String.valueOf(0L)), BigInteger::add);


                BigDecimal btcInUSDRate;
                try {

                    String yesterday = ZonedDateTime.now().minusDays(1).format(DateTimeFormatter.ISO_DATE);
                    btcInUSDRate = service.getUSDPriceForBTC(yesterday);
                } catch (JsonProcessingException e) {
                    System.err.println("failed to get btcInUSD rate" + e.getMessage());
                    return;
                }

                if(btcInUSDRate.compareTo(BigDecimal.valueOf(0L)) == -1){
                    System.err.println("failed to get btcInUSD rate");
                    return;
                }

                Map<String, String> collectedData = new HashMap<>(1);
                BigDecimal txVolumeInBTC = new BigDecimal(txVolumeInSatoshi).multiply(new BigDecimal(Math.pow(10, -8)));
                BigInteger txVolumeInUSD = btcInUSDRate.multiply(txVolumeInBTC).toBigInteger();
                collectedData.put("transaction.value.in.dollars", String.valueOf(txVolumeInUSD));
                dataCollectionRequest.collectData(collectedData, dataCollectionStatus -> {
                    if (dataCollectionStatus.getCode() != 0) {
                        System.err.println("failed to provide data. " + dataCollectionStatus.getDescription());
                    } else {
                        System.out.println("Successfully provided data");
                    }
                });
            }
        });
    };

    private static Optional<TransactionHandler> getTransactionHandler(SignRequest signRequest) {
        if (btcHandler.canDecode(signRequest.rawTransactions)){
            return Optional.ofNullable(btcHandler);
        }

        System.err.println("cannot decode raw TX");
        return Optional.empty();
    }


    @JsonIgnoreProperties(ignoreUnknown = true)
    @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY)
    public static class SignRequest {
        public List<String> rawTransactions = new ArrayList<>();
        public List<String> dataToSign = new ArrayList<>();
        public List<String> publicKeys = new ArrayList<>();
    }
}
