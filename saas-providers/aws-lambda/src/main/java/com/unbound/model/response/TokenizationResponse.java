package com.unbound.model.response;


public class TokenizationResponse {


    private String uid;

    private String value;
    private String tweak;

    private String error;

    /**
     * authorization
     **/
    public TokenizationResponse uid(String uid) {
        this.uid = uid;
        return this;
    }

    public String getUid() {
        return uid;
    }

    public void setUid(String uid) {
        this.uid = uid;
    }

    public TokenizationResponse token(String value) {
        this.value = value;
        return this;
    }

    public String getValue() {
        return value;
    }

    public void setValue(String value) {
        this.value = value;
    }

    public TokenizationResponse tweak(String tweak) {
        this.tweak = tweak;
        return this;
    }

    public String getTweak() {
        return tweak;
    }

    public void setTweak(String tweak) {
        this.tweak = tweak;
    }

    public TokenizationResponse error(String value) {
        this.error = error;
        return this;
    }

    public String getError() {
        return error;
    }

    public void setError(String error) {
        this.error = error;
    }
}
