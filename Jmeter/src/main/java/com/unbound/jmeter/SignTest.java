package com.unbound.jmeter;

import java.security.PrivateKey;
import java.security.Signature;

public abstract class SignTest extends AbstractTest {

    protected byte[] sign(byte[] in, String algorithm) {
        try {
            Signature sign = Signature.getInstance(algorithm, UNBOUND_PROVIDER);

            sign.initSign((PrivateKey) key);
            sign.update(in);
            return sign.sign();
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}
