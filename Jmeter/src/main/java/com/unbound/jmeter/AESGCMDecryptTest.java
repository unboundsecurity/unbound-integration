package com.unbound.jmeter;

import org.apache.jmeter.protocol.java.sampler.JavaSamplerContext;

import javax.crypto.Cipher;

public class AESGCMDecryptTest extends AESGCMTest {

    @Override
    public void setupTest(JavaSamplerContext context) {
        super.setupTest(context);

        encryptedData = encrypt();
    }

    @Override
    protected void crypt() {
        decrypt();
    }

    protected byte[] decrypt() {
        return crypt(Cipher.DECRYPT_MODE, encryptedData);
    }
}
