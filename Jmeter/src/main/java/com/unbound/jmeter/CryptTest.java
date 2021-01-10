package com.unbound.jmeter;

import com.dyadicsec.provider.DYCryptoProvider;
import com.unbound.provider.UBCryptoProvider;
import org.apache.jmeter.protocol.java.sampler.AbstractJavaSamplerClient;
import org.apache.jmeter.protocol.java.sampler.JavaSamplerContext;
import org.apache.jmeter.samplers.SampleResult;

import javax.crypto.Cipher;
import javax.crypto.spec.GCMParameterSpec;
import java.security.Key;
import java.security.KeyStore;
import java.security.Security;

public abstract class CryptTest extends AbstractJavaSamplerClient {

    static final String AES_GCM_PADDING = "AES/GCM/NoPadding";
    private static int IV_SIZE = 12;
    private static int TAG_LEN_IN_BITS = 8 * 12;

    Key key;

    byte[] data = new byte[32];
    byte[] iv = new byte[IV_SIZE];
    GCMParameterSpec gcmParameterSpec = new GCMParameterSpec(TAG_LEN_IN_BITS, iv);

    protected abstract void crypt();

    protected byte[] crypt(int mode, byte[] in) {
        try {
            Cipher cipher = Cipher.getInstance(AES_GCM_PADDING, "DYADIC");
            cipher.init(mode, key, gcmParameterSpec);
            return cipher.doFinal(in);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    protected byte[] encrypt() {
        return crypt(Cipher.ENCRYPT_MODE,data);
    }

    @Override
    public void setupTest(JavaSamplerContext context) {
        UBCryptoProvider.allowPrivateKeyWithoutCertificate(true);
        DYCryptoProvider provider = new DYCryptoProvider();
        Security.addProvider(provider);
        try {
            KeyStore keyStore = KeyStore.getInstance("PKCS11", "DYADIC");
            keyStore.load(null);
            key = keyStore.getKey("aes-256-key", null);
        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public SampleResult runTest(JavaSamplerContext javaSamplerContext) {
        SampleResult result = new SampleResult();
        result.sampleStart();

        crypt();

        result.sampleEnd();
        result.setSuccessful(true);
        return result;
    }
}