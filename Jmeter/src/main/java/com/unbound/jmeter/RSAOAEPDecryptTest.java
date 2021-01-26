package com.unbound.jmeter;

import org.apache.jmeter.protocol.java.sampler.JavaSamplerContext;

import javax.crypto.Cipher;
import javax.crypto.spec.OAEPParameterSpec;
import javax.crypto.spec.PSource;
import java.security.KeyFactory;
import java.security.interfaces.RSAPrivateCrtKey;
import java.security.interfaces.RSAPublicKey;
import java.security.spec.MGF1ParameterSpec;
import java.security.spec.RSAPublicKeySpec;

public class RSAOAEPDecryptTest extends CryptTest {

    private static final String RSA_OAEPWITHSHA_PADDING = "RSA/ECB/OAEPWITHSHA-256ANDMGF1PADDING";
    private final OAEPParameterSpec oaepParameterSpec = new OAEPParameterSpec("SHA-256", "MGF1", MGF1ParameterSpec.SHA256, PSource.PSpecified.DEFAULT);
    private RSAPublicKey publicKey;

    private static final String KEY_NAME = "rsa-2048-key";


    @Override
    public void setupTest(JavaSamplerContext context) {
        super.setupTest(context);

        try {
            KeyFactory keyFactory = KeyFactory.getInstance("RSA");
            publicKey = (RSAPublicKey) keyFactory.generatePublic(
                    new RSAPublicKeySpec(((RSAPrivateCrtKey) key).getModulus(), ((RSAPrivateCrtKey) key).getPublicExponent()));
        } catch (Exception e) {
            throw new RuntimeException(e);
        }

        encryptedData = encrypt();
    }

    @Override
    protected void doIt() {
        decrypt();
    }

    @Override
    protected String getKeyName() {
        return KEY_NAME;
    }

    @Override
    protected int getDataSize() {
        return 32;
    }

    protected byte[] encrypt() {
        return crypt(publicKey, Cipher.ENCRYPT_MODE, data, UNBOUND_PROVIDER, RSA_OAEPWITHSHA_PADDING, oaepParameterSpec);
    }

    protected byte[] decrypt() {
        return crypt(Cipher.DECRYPT_MODE, encryptedData, RSA_OAEPWITHSHA_PADDING, oaepParameterSpec);
    }
}
