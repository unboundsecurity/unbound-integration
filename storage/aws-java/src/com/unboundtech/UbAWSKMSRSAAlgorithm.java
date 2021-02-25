package com.unboundtech;

import com.amazonaws.services.kms.model.DecryptRequest;
import com.amazonaws.services.kms.model.DecryptResult;
import com.amazonaws.services.kms.model.GenerateDataKeyRequest;
import com.amazonaws.services.kms.model.GenerateDataKeyResult;
import com.dyadicsec.provider.RSAPrivateKey;

import javax.crypto.Cipher;
import javax.crypto.SecretKey;
import java.nio.ByteBuffer;
import java.security.GeneralSecurityException;
import java.security.KeyFactory;
import java.security.interfaces.RSAPublicKey;
import java.security.spec.RSAPublicKeySpec;

public class UbAWSKMSRSAAlgorithm extends UbAWSKMSAlgorithm<RSAPrivateKey> {

    static final String RSA_PADDING = "RSA/ECB/PKCS1Padding";

    /**
     * Generates and encrypts the data encryption key using the EKM RSA public key.
     *
     * @param request
     * @param sessionKey
     * @param masterKey
     * @return data encryption key, session key cipher and the master key uid
     */
    @Override
    GenerateDataKeyResult generateDataKey(GenerateDataKeyRequest request, SecretKey sessionKey, RSAPrivateKey masterKey) throws GeneralSecurityException {
        KeyFactory keyFactory = KeyFactory.getInstance("RSA");
        RSAPublicKey rsaPublicKey = (RSAPublicKey) keyFactory.generatePublic(
                new RSAPublicKeySpec(masterKey.getModulus(), masterKey.getPublicExponent()));
        Cipher cipher = Cipher.getInstance(RSA_PADDING, "DYADIC");
        cipher.init(Cipher.ENCRYPT_MODE, rsaPublicKey);
        byte[] enc = cipher.doFinal(sessionKey.getEncoded());
        return new GenerateDataKeyResult()
                .withPlaintext(ByteBuffer.wrap(sessionKey.getEncoded()))
                .withCiphertextBlob(ByteBuffer.wrap(enc))
                .withKeyId(request.getKeyId());
    }

    /**
     * Decrypts  the data encryption key using the EKM RSA private key.
     *
     * @param request
     * @param masterKey
     * @return data encryption key
     */
    @Override
    DecryptResult decrypt(DecryptRequest request, RSAPrivateKey masterKey) throws GeneralSecurityException {
        Cipher cipher = Cipher.getInstance(RSA_PADDING, "DYADIC");
        cipher.init(Cipher.DECRYPT_MODE, masterKey);
        byte[] dec = cipher.doFinal(request.getCiphertextBlob().array());
        return new DecryptResult().withPlaintext(ByteBuffer.wrap(dec));
    }
}
