package com.unbound.jmeter;

import javax.crypto.Cipher;
import java.security.Key;
import java.security.spec.AlgorithmParameterSpec;

public abstract class CryptTest extends AbstractTest {

    protected byte[] encryptedData;

    protected byte[] crypt(int mode, byte[] in, String transformation, AlgorithmParameterSpec spec) {
        return crypt(key, mode, in, AbstractTest.UNBOUND_PROVIDER, transformation, spec);
    }

    protected byte[] crypt(Key key, int mode, byte[] in, String provider, String transformation, AlgorithmParameterSpec spec) {
        try {
            Cipher cipher = Cipher.getInstance(transformation, provider);

            cipher.init(mode, key, spec);
            return cipher.doFinal(in);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}
