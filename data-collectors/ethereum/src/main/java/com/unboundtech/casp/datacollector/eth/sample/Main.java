package com.unboundtech.casp.datacollector.eth.sample;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
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

import java.math.BigInteger;
import java.util.*;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledThreadPoolExecutor;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

public class Main {
    private static final ScheduledExecutorService poolExecutor = new ScheduledThreadPoolExecutor(1);
    public static EthereumTransactionHandler ethRopstenHandler = new EthereumRopstenTransactionHandler();
    public static EthereumTransactionHandler ethHandler = new EthereumMainNetTransactionHandler();

    public static void main(String[] args) throws Exception {
        CommandLineParser parser = new DefaultParser();
        Options options = new Options();
        options.addOption("i", "dataCollectorId", true, "Data collector ID");
        options.addOption("c", "activation-code", true, "The activation code provided by the CASP server");
        options.addOption("w", "keystorepass", true, "Keystore password");
        options.addOption("v", "verbose", false, "Enable verbose logging");
        options.addOption("u", "server-url", true, "CASP server url");
        options.addOption("k", "insecure", false, "Allow connections without certificate verification");

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
        System.out.println("Data collector version: " + DataCollectorSdk.getInstance().sdkVersion());
        System.out.println("Data collector ID: " + dcId);
        boolean allowInsecureConnection = line.hasOption("k");
        KeyStoreStorage keyStoreStorage = new KeyStoreStorage(dcId, keyStorePassword);
        JavaRestClient javaRestClient = new JavaRestClient(line.getOptionValue("u"), allowInsecureConnection, keyStoreStorage, keyStorePassword);
        String serverUrl = line.getOptionValue("u");
        System.out.println("Server URL: " + serverUrl);
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
                    System.err.println("DC activation failed. " + status.getDescription());
                }
                latch.countDown();
            });
            latch.await();
            return;
        }
        System.out.println("Starting data collection");
        poolExecutor.scheduleAtFixedRate(Main::collectDataFromEthTx, 0, 10, TimeUnit.SECONDS);
        poolExecutor.awaitTermination(9999, TimeUnit.DAYS);
    }

    private static String getKeyStorePassword(CommandLine line) {
        if (line.hasOption("w")) {
            return line.getOptionValue("w");
        } else {
            return new String(System.console().readPassword("%s :  ", "Please, enter keystore password"));
        }
    }


    public static void collectDataFromEthTx() {
        DataCollectorSdk.getInstance().getDataCollectionRequest((getDataCollectionRequestStatus, dataCollectionRequest) -> {
            if (getDataCollectionRequestStatus.getCode() == CaspStatus.DY_ENO_ENTITY) {
                System.out.println(getDataCollectionRequestStatus.getDescription());
                return;
            }
            if (getDataCollectionRequestStatus.getCode() != 0) {
                System.err.println("failed to retrieve data collection request. " + getDataCollectionRequestStatus.getDescription());
                return;
            }
            System.out.println("data collection details: " + dataCollectionRequest.toString());
            SignRequest signRequest = Utils.fromJson(dataCollectionRequest.getSignRequest(), SignRequest.class);
            if (signRequest.rawTransactions.size() == 0) {
                System.err.println("'rawTransactions' are missing from sign request");
                System.exit(-1);
            }
            TransactionHandler handler = getTransactionHandler(signRequest);
            List<DetailedTransaction> detailedTransactions = null;
            try {
                detailedTransactions = handler.decode(signRequest.dataToSign, signRequest.rawTransactions, signRequest. publicKeys, Collections.emptySet());
            } catch (BadTransactionException e) {
                System.err.println("failed to decode raw tx");
                System.exit(-1);
            }

            List<String> assets =  detailedTransactions.stream()
                    .map(detailedTransaction -> detailedTransaction.getAsset())
                    .collect(Collectors.toList());

            List<String> recipientAddresses =  detailedTransactions.stream()
                    .flatMap(detailedTransaction -> detailedTransaction.getDestinations().stream())
                    .collect(Collectors.toList());

            BigInteger totalAmount = detailedTransactions.stream()
                    .map(detailedTransaction -> detailedTransaction.getAmount())
                    .reduce(BigInteger::add).orElse(new BigInteger("0"));

            Map<String, String> collectedData = new HashMap<>(2);
            collectedData.put("contractAddress", assets.get(0));
            collectedData.put("recipientAddress", recipientAddresses.get(0));
            collectedData.put("totalAmount", String.valueOf(totalAmount));
            dataCollectionRequest.collectData(collectedData, dataCollectionStatus -> {
                if (dataCollectionStatus.getCode() != 0) {
                    System.err.println("failed to provide data. " + dataCollectionStatus.getDescription());
                } else {
                    System.out.println("Successfully provided data");
                }
            });
        });
    };
    private static TransactionHandler getTransactionHandler(SignRequest signRequest) {
        if(ethRopstenHandler.canDecode(signRequest.rawTransactions)){
            return ethRopstenHandler;
        }

        if(ethHandler.canDecode(signRequest.rawTransactions)){
            return ethHandler;
        }

        System.err.println("cannot decode raw TX");
        System.exit(-1);
        return null;
    }
    @JsonIgnoreProperties(ignoreUnknown = true)
    @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY)
    public static class SignRequest {
        public List<String> rawTransactions = new ArrayList<>();
        public List<String> dataToSign = new ArrayList<>();
        public List<String> publicKeys = new ArrayList<>();
    }
}
