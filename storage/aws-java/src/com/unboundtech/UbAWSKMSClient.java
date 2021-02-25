package com.unboundtech;

import com.amazonaws.services.kms.AWSKMSClient;
import com.amazonaws.services.kms.model.*;
import com.dyadicsec.provider.DYCryptoProvider;

import javax.crypto.KeyGenerator;
import javax.crypto.SecretKey;
import java.io.IOException;
import java.security.*;
import java.security.cert.CertificateException;

public class UbAWSKMSClient<T extends Key> extends AWSKMSClient {

    private final String partition;
    private final UbAWSKMSAlgorithm ubAWSKMSAlgorithm;

    public UbAWSKMSClient(String partition, UbAWSKMSAlgorithm ubAWSKMSAlgorithm) {
        this.partition = partition;
        this.ubAWSKMSAlgorithm = ubAWSKMSAlgorithm;
    }

    /**
     * Loads the EKM key store
     *
     * @return
     * @throws CertificateException
     * @throws NoSuchAlgorithmException
     * @throws IOException
     * @throws NoSuchProviderException
     * @throws KeyStoreException
     */
    KeyStore loadKeyStore() throws GeneralSecurityException, IOException {
        Provider provider = new DYCryptoProvider(partition);
        Security.addProvider(provider);

        KeyStore keyStore = KeyStore.getInstance("PKCS11", "DYADIC");
        keyStore.load(null);

        return keyStore;
    }

    /**
     * Generates the data encryption key
     *
     * @return
     * @throws NoSuchAlgorithmException
     */
    SecretKey generateSessionKey() throws NoSuchAlgorithmException {
        KeyGenerator symKeyGenerator = KeyGenerator.getInstance("AES");
        symKeyGenerator.init(256);
        return symKeyGenerator.generateKey();
    }

    @Override
    public GenerateDataKeyResult generateDataKey(GenerateDataKeyRequest request) {
        try {
            KeyStore keyStore = loadKeyStore();
            T masterKey = (T) keyStore.getKey(request.getKeyId(), null);
            return ubAWSKMSAlgorithm.generateDataKey(request, generateSessionKey(), masterKey);
        } catch (GeneralSecurityException | IOException e) {
            e.printStackTrace();
            throw new AWSKMSException(e.getMessage());
        }
    }

    @Override
    public DecryptResult decrypt(DecryptRequest request) {
        try {
            KeyStore keyStore = loadKeyStore();
            T masterKey = (T) keyStore.getKey(request.getEncryptionContext().get("kms_cmk_id"), null);
            return ubAWSKMSAlgorithm.decrypt(request, masterKey);
        } catch (GeneralSecurityException | IOException e) {
            e.printStackTrace();
            throw new AWSKMSException(e.getMessage());
        }
    }
}
