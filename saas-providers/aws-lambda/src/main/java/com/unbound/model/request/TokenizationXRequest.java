package com.unbound.model.request;

import com.unbound.model.DataType;

import java.util.ArrayList;
import java.util.List;

public class TokenizationXRequest{


    protected
    String keyId = null;

    private
    List<String> valueItems = new ArrayList<String>();

    protected
    String tweak = null;

    protected
    DataType dataType = null;

    protected
    String format;

    protected
    Integer maxSize;

    public String getKeyId() {
        return keyId;
    }
    public void setKeyId(String keyId) {
        this.keyId = keyId;
    }
    public TokenizationXRequest keyId(String keyId){
        setKeyId(keyId);
        return this;
    }

    public TokenizationXRequest valueItems(List<String> valueItems){
        this.valueItems = valueItems;
        return this;
    }

    public List<String> getValueItems() {
        return valueItems;
    }
    public void setValueItems(List<String> valueItems) {
        this.valueItems = valueItems;
    }

    public TokenizationXRequest tweak(String tweak){
        this.tweak = tweak;
        return this;
    }

    public String getTweak() {
        return tweak;
    }

    public void setTweak(String tweak) {
        this.tweak = tweak;
    }

    public TokenizationXRequest dataType(DataType dataType){
        this.dataType = dataType;
        return this;
    }

    public DataType getDataType() {
        return dataType;
    }

    public void setDataType(DataType dataType) {
        this.dataType = dataType;
    }

    public TokenizationXRequest format(String format){
        this.format = format;
        return this;
    }

    public String getFormat() {
        return format;
    }

    public void setFormat(String format) {
        this.format = format;
    }

    public TokenizationXRequest maxSize(Integer maxSize){
        this.maxSize = maxSize;
        return this;
    }

    public Integer getMaxSize() {
        return maxSize;
    }

    public void setMaxSize(Integer maxSize) {
        this.maxSize = maxSize;
    }

}