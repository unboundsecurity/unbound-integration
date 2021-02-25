package com.unbound.jmeter;

public class ECDSATest extends SignTest {

    private static final String ECDSA_PADDING = "SHA256WithECDSA";
    private static final String KEY_NAME = "ecdsa-p256-key";

    @Override
    protected void doIt() {
        sign(data, ECDSA_PADDING);
    }

    @Override
    protected String getKeyName() {
        return KEY_NAME;
    }

    @Override
    protected int getDataSize() {
        return 32;
    }
}
