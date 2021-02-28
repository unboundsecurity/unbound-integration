package com.unboundtech;

import com.amazonaws.services.kms.model.DecryptRequest;
import com.amazonaws.services.kms.model.DecryptResult;
import com.amazonaws.services.kms.model.GenerateDataKeyRequest;
import com.amazonaws.services.kms.model.GenerateDataKeyResult;

import javax.crypto.Cipher;
import javax.crypto.SecretKey;
import javax.crypto.spec.GCMParameterSpec;
import java.nio.ByteBuffer;
import java.security.GeneralSecurityException;
import java.security.SecureRandom;
import java.util.Arrays;

public class UbAWSKMSGCMAlgorithm extends UbAWSKMSAlgorithm<SecretKey> {

    static final String AES_GCM_PADDING = "AES/GCM/NoPadding";

    private static int SESSION_KEY_SIZE = 32;
    private static int IV_SIZE = 12;
    private static int TAG_LEN_IN_BYTES = 12;
    private static int TAG_LEN_IN_BITS = 8 * TAG_LEN_IN_BYTES;

    /**
     * Generates and encrypts the data encryption key using the EKM symmetric eky with GCM encryption
     *
     * @param request
     * @param sessionKey
     * @param masterKey
     * @return data encryption key, session key cipher and the master key uid
     */
    @Override
    GenerateDataKeyResult generateDataKey(GenerateDataKeyRequest request, SecretKey sessionKey, SecretKey masterKey) throws GeneralSecurityException {
        byte[] iv = new byte[IV_SIZE];
        new SecureRandom().nextBytes(iv);
        GCMParameterSpec gcmParameterSpec = new GCMParameterSpec(TAG_LEN_IN_BITS, iv);
        Cipher cipher = Cipher.getInstance(AES_GCM_PADDING, "DYADIC");
        cipher.init(Cipher.ENCRYPT_MODE, masterKey, gcmParameterSpec);
        byte[] enc = cipher.doFinal(sessionKey.getEncoded());
        ByteBuffer ciphertextBlob = ByteBuffer.allocate(iv.length + enc.length);
        ciphertextBlob.put(enc);
        ciphertextBlob.put(iv);
        return new GenerateDataKeyResult()
                .withPlaintext(ByteBuffer.wrap(sessionKey.getEncoded()))
                .withCiphertextBlob(ciphertextBlob)
                .withKeyId(request.getKeyId());
    }

    /**
     * Decrypts the data encryption key using the EKM symmetric eky with GCM encryption
     *
     * @param request
     * @param masterKey
     * @return data encryption key
     */
    @Override
    DecryptResult decrypt(DecryptRequest request, SecretKey masterKey) throws GeneralSecurityException {
        byte[] ciphertextBlob = request.getCiphertextBlob().array();
        byte[] enc = Arrays.copyOfRange(ciphertextBlob, 0, SESSION_KEY_SIZE + TAG_LEN_IN_BYTES);
        byte[] iv = Arrays.copyOfRange(ciphertextBlob, SESSION_KEY_SIZE + TAG_LEN_IN_BYTES, SESSION_KEY_SIZE + TAG_LEN_IN_BYTES + IV_SIZE);
        GCMParameterSpec gcmParameterSpec = new GCMParameterSpec(TAG_LEN_IN_BITS, iv);
        Cipher cipher = Cipher.getInstance(AES_GCM_PADDING, "DYADIC");
        cipher.init(Cipher.DECRYPT_MODE, masterKey, gcmParameterSpec);
        byte[] dec = cipher.doFinal(enc);
        return new DecryptResult().withPlaintext(ByteBuffer.wrap(dec));
    }
}
