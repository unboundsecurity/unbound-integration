package com.unboundtech.casp.datacollector.coinmetrics.sample;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.core.util.JacksonFeature;

import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.GenericType;
import javax.ws.rs.core.HttpHeaders;
import java.math.BigInteger;
import java.time.ZonedDateTime;
import java.util.Comparator;
import java.util.List;

import static java.util.concurrent.TimeUnit.*;

public class CoinMetricsQueryService {
    private final String accessToken;
    private final WebTarget assetMetricsTarget;


    public CoinMetricsQueryService(String accessToken) {
        this.accessToken = accessToken;
        Client client = ClientBuilder.newBuilder()
                .connectTimeout(3, SECONDS)
                .register(JacksonFeature.class)
                .build();

        assetMetricsTarget = client.target("https://api.coinmetrics.io/v4/timeseries/asset-metrics");
    }

    //assets=btc&metrics=PriceUSD&start_time=2021-03-20&frequency=1d&pretty=true&sort=time&api_key=zZQZUSMowLMGFMPXpOdB

    public BigInteger getUSDPriceForBTC(String startTime, String frequency) {
        return assetMetricsTarget
                .queryParam("assets", "btc")
                .queryParam("metrics", "PriceUSD")
                .queryParam("start_time", startTime)
                .queryParam("frequency", frequency)
                .queryParam("pretty", true)
                .queryParam("sort", "time")
                .queryParam("pretty", true)
                .queryParam("api_key", accessToken)
                .request()
                .header(HttpHeaders.AUTHORIZATION, accessToken)
                .get(new GenericType<List<CoinMetricsResponse>>(){})
                .stream()
                .max(Comparator.comparing(CoinMetricsResponse::getTime))
                .get()
                .getPriceUSD();

    }



    @JsonIgnoreProperties(ignoreUnknown = true)
    @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY)
    public static class CoinMetricsResponse {
        public String asset;
        public ZonedDateTime time;
        public BigInteger priceUSD;

        public CoinMetricsResponse(String asset, String time, String priceUSD){
            this.asset = asset;
            this.time = ZonedDateTime.parse(time);
            this.priceUSD = new BigInteger(priceUSD);
        }

        public String getAsset() {
            return asset;
        }

        public ZonedDateTime getTime() {
            return time;
        }

        public BigInteger getPriceUSD() {
            return priceUSD;
        }
    }

}
