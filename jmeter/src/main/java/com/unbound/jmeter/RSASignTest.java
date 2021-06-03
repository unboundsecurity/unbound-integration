package com.unbound.jmeter;

import com.dyadicsec.provider.DYCryptoProvider;
import com.unbound.provider.UBCryptoProvider;
import org.apache.jmeter.protocol.java.sampler.JavaSamplerContext;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.security.*;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.concurrent.ThreadLocalRandom;

public class RSASignTest extends SignTest{

    private static final Logger log = LoggerFactory.getLogger(RSASignTest.class);
    private static final String SIGN_ALGORITHM = "RSA";
    private static final String RSA_PADDING = "SHA256WithRSA";
    private static final String KEY_NAME = "rsa-2048-key";
    private static final String KEY_STORE_PASSWORD = "Password1!";

    private String keyName = null;

    @Override
    public void setupTest(JavaSamplerContext context) {
        UBCryptoProvider.allowPrivateKeyWithoutCertificate(true);
        DYCryptoProvider provider = new DYCryptoProvider();
        Security.addProvider(provider);

        try {
            KeyStore keyStore = KeyStore.getInstance(UNBOUND_PROVIDER_TYPE,  UNBOUND_PROVIDER);
            keyStore.load(null,KEY_STORE_PASSWORD.toCharArray());

            boolean createKeys = Boolean.valueOf(context.getJMeterVariables().get("createKeys"));
            if (createKeys) {
                KeyPairGenerator kpg = KeyPairGenerator.getInstance(SIGN_ALGORITHM, provider);
                kpg.initialize(2048);
                KeyPair kp = kpg.generateKeyPair();
                int randomNum = ThreadLocalRandom.current().nextInt(0, Integer.MAX_VALUE);
                keyName = KEY_NAME + "-" + randomNum;
                keyStore.setEntry(getKeyName(), new UBCryptoProvider.KeyEntry(kp.getPrivate()), null);
            } else {
                String threadName = Thread.currentThread().getName();
                int threadNum = Integer.parseInt(threadName.substring(threadName.lastIndexOf('-')+1));
                ArrayList<String> keysAliases = KeysHolder.getInstance().getKeysAliases();
                keyName = keysAliases.get(threadNum-1);
            }
            key = keyStore.getKey(getKeyName(), null);
            log.info("thread : " + Thread.currentThread().getName() + " keyName: "+ keyName);

        } catch (Exception e) {
            log.error("**** Problem with key of thread'{}'", Thread.currentThread().getName());
            throw new RuntimeException(e);
        }

        data = new byte[getDataSize()];
    }

    @Override
    public void teardownTest(JavaSamplerContext context) {
        try {
            boolean deleteKeys = Boolean.valueOf(context.getJMeterVariables().get("deleteKeys"));
            if (deleteKeys) {
                KeyStore keyStore = KeyStore.getInstance(UNBOUND_PROVIDER_TYPE, UNBOUND_PROVIDER);
                keyStore.load(null, KEY_STORE_PASSWORD.toCharArray());
                keyStore.deleteEntry(getKeyName());
            }
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
        KeysHolder.releaseInstance();
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
