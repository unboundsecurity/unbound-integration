package com.unbound.jmeter;

import org.apache.jmeter.protocol.java.sampler.JavaSamplerContext;

import javax.crypto.KeyAgreement;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.spec.ECGenParameterSpec;

public class ECDHTest extends AbstractTest {

    private KeyPair sunPair;
    private static final String KEY_NAME = "ecdh-p256-key";

    @Override
    public void setupTest(JavaSamplerContext context) {
        super.setupTest(context);

        try {
            KeyPairGenerator sunKeyGen = KeyPairGenerator.getInstance("EC");
            sunKeyGen.initialize(new ECGenParameterSpec("secp256r1"));
            sunPair = sunKeyGen.generateKeyPair();
        } catch (Exception e) {
            throw new RuntimeException(e);
        }

    }

    @Override
    protected void doIt() {
        try {
            KeyAgreement keyAgreement = KeyAgreement.getInstance("ECDH", UNBOUND_PROVIDER);
            keyAgreement.init(key);
            keyAgreement.doPhase(sunPair.getPublic(), true);
            keyAgreement.generateSecret();
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
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
