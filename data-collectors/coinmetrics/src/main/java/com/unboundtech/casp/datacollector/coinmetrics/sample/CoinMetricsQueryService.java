package com.unboundtech.casp.datacollector.coinmetrics.sample;

import com.fasterxml.jackson.annotation.JsonAutoDetect;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.core.util.JacksonFeature;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.datatype.jdk8.Jdk8Module;

import javax.ws.rs.client.Client;
import javax.ws.rs.client.ClientBuilder;
import javax.ws.rs.client.WebTarget;
import javax.ws.rs.core.HttpHeaders;
import java.math.BigDecimal;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

import static java.util.concurrent.TimeUnit.*;

public class CoinMetricsQueryService {
    private final String accessToken;
    private final WebTarget assetMetricsTarget;
    private static final ObjectMapper mapper;

    static {
        mapper = new ObjectMapper();
        mapper.registerModule(new Jdk8Module());
    }


    public CoinMetricsQueryService(String accessToken) {
        this.accessToken = accessToken;
        Client client = ClientBuilder.newBuilder()
                .connectTimeout(3, SECONDS)
                .register(JacksonFeature.class)
                .build();

        assetMetricsTarget = client.target("https://api.coinmetrics.io/v4/timeseries/asset-metrics");
    }

    //https://docs.coinmetrics.io/api/v4#operation/getTimeseriesAssetMetrics
    public BigDecimal getUSDPriceForBTC(String startTime) throws JsonProcessingException {
        String coinMetricsDataString =  assetMetricsTarget
                .queryParam("assets", "btc")
                .queryParam("metrics", "PriceUSD")
                .queryParam("start_time", startTime)
                .queryParam("frequency", "1d")
                .queryParam("pretty", true)
                .queryParam("sort", "time")
                .queryParam("pretty", true)
                .queryParam("api_key", accessToken)
                .request()
                .header(HttpHeaders.AUTHORIZATION, accessToken)
                .get(String.class);

        CoinMetricsData coinMetricsData = mapper.readValue(coinMetricsDataString, new TypeReference<CoinMetricsData>(){});

        if(coinMetricsData != null && !coinMetricsData.data.isEmpty()) {
            Comparator<CoinMetricsResponse> coinMetricsResponseComparator
                    = Comparator.comparing(
                    CoinMetricsResponse::getTime, (t1, t2) -> {
                        ZonedDateTime time1 = ZonedDateTime.parse(t1, DateTimeFormatter.ISO_ZONED_DATE_TIME);
                        ZonedDateTime time2 = ZonedDateTime.parse(t2, DateTimeFormatter.ISO_ZONED_DATE_TIME);
                        return time1.compareTo(time2);
                    });
            return new BigDecimal(
                    coinMetricsData.data
                    .stream()
                    .max(coinMetricsResponseComparator)
                    .get()
                    .getPriceUSD()
            );
        }else {
            return new BigDecimal(String.valueOf(1L)).negate();
        }

    }



    @JsonIgnoreProperties(ignoreUnknown = true)
    @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY)
    public static class CoinMetricsResponse {
        public String asset;
        public String time;
        public String PriceUSD;

        public CoinMetricsResponse(){}

        public String getAsset() {
            return asset;
        }

        public String getTime() {
            return time;
        }

        public String getPriceUSD() {
            return PriceUSD;
        }
    }

    @JsonIgnoreProperties(ignoreUnknown = true)
    @JsonAutoDetect(fieldVisibility = JsonAutoDetect.Visibility.ANY)
    public static class CoinMetricsData {
        public List<CoinMetricsResponse> data = new ArrayList<>();
    }

}
