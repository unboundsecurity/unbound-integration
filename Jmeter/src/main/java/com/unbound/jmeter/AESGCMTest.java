package com.unbound.jmeter;

import javax.crypto.Cipher;
import javax.crypto.spec.GCMParameterSpec;

public abstract class AESGCMTest extends CryptTest {

    private static final String AES_GCM_PADDING = "AES/GCM/NoPadding";
    private static final int IV_SIZE = 12;
    private static final int TAG_LEN_IN_BITS = 8 * 12;
    private static final String KEY_NAME = "aes-256-key";

    final byte[] iv = new byte[IV_SIZE];
    final GCMParameterSpec gcmParameterSpec = new GCMParameterSpec(TAG_LEN_IN_BITS, iv);

    @Override
    protected void doIt() {
        crypt();
    }

    @Override
    protected String getKeyName() {
        return KEY_NAME;
    }

    @Override
    protected int getDataSize() {
        return 32;
    }

    protected abstract void crypt();

    protected byte[] crypt(int mode, byte[] in) {
        return crypt(mode, in, AES_GCM_PADDING, gcmParameterSpec);
    }

    protected byte[] encrypt() {
        return crypt(Cipher.ENCRYPT_MODE, data);
    }
}