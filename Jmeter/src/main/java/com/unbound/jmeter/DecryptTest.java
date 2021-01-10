package com.unbound.jmeter;

import org.apache.jmeter.protocol.java.sampler.JavaSamplerContext;

import javax.crypto.Cipher;

public class DecryptTest extends CryptTest {

    byte[] encryptedData;

    protected byte[] decrypt() {
        return crypt(Cipher.DECRYPT_MODE, encryptedData);
    }

    @Override
    public void setupTest(JavaSamplerContext context) {
        super.setupTest(context);

        encryptedData = encrypt();
    }

    @Override
    protected void crypt() {
        decrypt();
    }
}
