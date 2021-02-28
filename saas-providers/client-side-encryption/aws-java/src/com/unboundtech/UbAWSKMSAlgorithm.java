package com.unboundtech;

import com.amazonaws.services.kms.model.DecryptRequest;
import com.amazonaws.services.kms.model.DecryptResult;
import com.amazonaws.services.kms.model.GenerateDataKeyRequest;
import com.amazonaws.services.kms.model.GenerateDataKeyResult;

import javax.crypto.SecretKey;
import java.security.GeneralSecurityException;

public abstract class UbAWSKMSAlgorithm<T> {

    /**
     * Returns a data encryption key that you can use in your application to encrypt data locally.
     * @param request
     * @param sessionKey
     * @param masterKey
     * @return
     */
    abstract GenerateDataKeyResult generateDataKey(GenerateDataKeyRequest request, SecretKey sessionKey, T masterKey) throws GeneralSecurityException;

    /**
     * Decrypts ciphertext.
     * @param request
     * @param masterKey
     * @return
     */
    abstract DecryptResult decrypt(DecryptRequest request, T masterKey) throws GeneralSecurityException;

}
