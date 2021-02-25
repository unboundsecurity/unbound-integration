package com.unbound.jmeter;

import com.dyadicsec.provider.DYCryptoProvider;
import com.unbound.provider.UBCryptoProvider;
import org.apache.jmeter.protocol.java.sampler.AbstractJavaSamplerClient;
import org.apache.jmeter.protocol.java.sampler.JavaSamplerContext;
import org.apache.jmeter.samplers.SampleResult;

import java.security.Key;
import java.security.KeyStore;
import java.security.Security;

public abstract class AbstractTest extends AbstractJavaSamplerClient {


    protected static final String UNBOUND_PROVIDER_TYPE = "PKCS11";
    protected static final String UNBOUND_PROVIDER = "DYADIC";

    protected Key key;
    byte[] data;

    @Override
    public void setupTest(JavaSamplerContext context) {

        UBCryptoProvider.allowPrivateKeyWithoutCertificate(true);
        DYCryptoProvider provider = new DYCryptoProvider();
        Security.addProvider(provider);

        try {
            KeyStore keyStore = KeyStore.getInstance(UNBOUND_PROVIDER_TYPE, UNBOUND_PROVIDER);
            keyStore.load(null);
            key = keyStore.getKey(getKeyName(), null);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }

        data = new byte[getDataSize()];
    }

    @Override
    public SampleResult runTest(JavaSamplerContext javaSamplerContext) {
        SampleResult result = new SampleResult();
        result.sampleStart();

        doIt();

        result.sampleEnd();
        result.setSuccessful(true);
        return result;
    }

    protected abstract void doIt();

    protected abstract String getKeyName();

    protected abstract int getDataSize();

}
