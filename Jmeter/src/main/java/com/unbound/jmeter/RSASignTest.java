package com.unbound.jmeter;

import com.dyadicsec.provider.DYCryptoProvider;
import org.apache.jmeter.protocol.java.sampler.JavaSamplerContext;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.security.*;

public class RSASignTest extends SignTest{

    private static final Logger log = LoggerFactory.getLogger(RSASignTest.class);
    private static final String SIGN_ALGORITHM = "RSA";
    private static final String RSA_PADDING = "SHA256WithRSA";
    private static final String KEY_NAME = "rsa-2048-key";
    private static final String KEY_STORE_PASSWORD = "Password1!";

    private String keyName = KEY_NAME;

    @Override
    public void setupTest(JavaSamplerContext context) {

        DYCryptoProvider provider = new DYCryptoProvider();
        Security.addProvider(provider);

        try {
            KeyStore keyStore = KeyStore.getInstance(UNBOUND_PROVIDER_TYPE,  UNBOUND_PROVIDER);
            keyStore.load(null,KEY_STORE_PASSWORD.toCharArray());


            KeyPairGenerator kpg = KeyPairGenerator.getInstance(SIGN_ALGORITHM, provider);
            kpg.initialize(2048);
            KeyPair kp = kpg.generateKeyPair();
            keyName = KEY_NAME + "-" + Thread.currentThread().getName();
            keyStore.setEntry(getKeyName(), new DYCryptoProvider.KeyEntry(kp.getPrivate()), null);

            key = keyStore.getKey(getKeyName(), null);

        } catch (Exception e) {
            throw new RuntimeException(e);
        }

        data = new byte[getDataSize()];
    }

    @Override
    public void teardownTest(JavaSamplerContext context) {
        try {
            KeyStore keyStore = KeyStore.getInstance(UNBOUND_PROVIDER_TYPE,  UNBOUND_PROVIDER);
            keyStore.load(null,KEY_STORE_PASSWORD.toCharArray());
            keyStore.deleteEntry(getKeyName());
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
        log.info(this.getClass().getName() + ": teardownTest " + getKeyName());
    }

    @Override
    protected void doIt() {
        sign(data, RSA_PADDING);
    }

    @Override
    protected String getKeyName() {
        return keyName;
    }

    @Override
    protected int getDataSize() {
        return 32;
    }
}
