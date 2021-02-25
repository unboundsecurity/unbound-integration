package com.unboundtech.casp.datacollector.cyphertrace.sample;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import org.glassfish.jersey.jackson.JacksonFeature;

import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.HttpHeaders;
import java.util.concurrent.TimeUnit;

public class CipherTraceQueryService {
    private final String accessToken;

    private final WebTarget ethTarget;
    private final WebTarget btcTarget;


    public CipherTraceQueryService(String accessToken) {
        this.accessToken = accessToken;
        Client client = ClientBuilder.newBuilder()
                .connectTimeout(3, TimeUnit.SECONDS)
                .register(JacksonFeature.class)
                .build();

        ethTarget = client.target("https://rest.ciphertrace.com/aml/v1/eth/risk");
        btcTarget = client.target("https://rest.ciphertrace.com/aml/v1/btc/risk");
    }

    int getRiskForBitcoinAddress(String btcAddress) {
        return btcTarget
                .queryParam("address", btcAddress)
                .request()
                .header(HttpHeaders.AUTHORIZATION, accessToken)
                .get(CipherTraceResponse.class)
                .risk;
    }

    int getRiskForEthereumAddress(String ethAddress) {
        return ethTarget
                .queryParam("address", ethAddress)
                .request()
                .header(HttpHeaders.AUTHORIZATION, accessToken)
                .get(CipherTraceResponse.class)
                .risk;
    }

    @JsonIgnoreProperties(ignoreUnknown = true)
    @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY)
    public static class CipherTraceResponse {
        public int risk;
    }
}
