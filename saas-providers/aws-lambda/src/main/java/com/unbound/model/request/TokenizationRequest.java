package com.unbound.model.request;


import com.unbound.model.DataType;

public class TokenizationRequest{

    protected
    String keyId = null;

    protected
    String value = null;

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
    public TokenizationRequest keyId(String keyId){
        setKeyId(keyId);
        return this;
    }

    public TokenizationRequest value(String value){
        this.value = value;
        return this;
    }


    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        this.value = value;
    }

    public TokenizationRequest tweak(String tweak){
        this.tweak = tweak;
        return this;
    }

    public String getTweak() {
        return tweak;
    }

    public void setTweak(String tweak) {
        this.tweak = tweak;
    }

    public TokenizationRequest dataType(DataType dataType){
        this.dataType = dataType;
        return this;
    }

    public DataType getDataType() {
        return dataType;
    }

    public void setDataType(DataType dataType) {
        this.dataType = dataType;
    }

    public TokenizationRequest format(String format){
        this.format = format;
        return this;
    }

    public String getFormat() {
        return format;
    }

    public void setFormat(String format) {
        this.format = format;
    }

    public TokenizationRequest maxSize(Integer maxSize){
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
