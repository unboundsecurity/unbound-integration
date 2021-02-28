package com.unbound.quorum.encryption.ub;

import com.dyadicsec.provider.DYCryptoProvider;
import com.dyadicsec.provider.KeyGenSpec;
import com.dyadicsec.provider.KeyParameters;
import com.quorum.tessera.encryption.KeyPair;
import com.quorum.tessera.encryption.PrivateKey;
import com.quorum.tessera.encryption.PublicKey;
import com.quorum.tessera.encryption.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import javax.crypto.Cipher;
import javax.crypto.KeyAgreement;
import javax.crypto.spec.GCMParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import java.nio.ByteBuffer;
import java.security.*;
import java.security.interfaces.ECPublicKey;
import java.security.spec.AlgorithmParameterSpec;
import java.security.spec.ECGenParameterSpec;
import java.security.spec.ECPoint;
import java.security.spec.X509EncodedKeySpec;
import java.util.Arrays;

public class UnboundEncryptor implements Encryptor {

    private static final Logger LOGGER = LoggerFactory.getLogger(UnboundEncryptor.class);

    static final int AES_GCM_IV_LEN = 16;
    static final int AES_GCM_TAG_LEN = 16;
    static final int AES_KEY_LEN = 16;

    private final SecureRandom secureRandom = new SecureRandom();

    public UnboundEncryptor() throws EncryptorException {
        Provider provider = Security.getProvider("DYADIC");
        if (provider == null) {
            provider = new DYCryptoProvider();
            Security.addProvider(provider);
        }
    }

    private static byte[] longToBytes(long x) {
        ByteBuffer buffer = ByteBuffer.allocate(Long.BYTES);
        buffer.putLong(0, x);
        return buffer.array();
    }

    private static long bytesToLong(byte[] bytes) {
        ByteBuffer buffer = ByteBuffer.allocate(Long.BYTES);
        buffer.put(bytes, 0, bytes.length);
        buffer.flip();//need flip
        return buffer.getLong();
    }

    static byte[] ecPointToDer(ECPoint p) {
        byte[] out = new byte[67];
        out[0] = 4;
        out[1] = 65;
        out[2] = 4;

        byte[] bin = p.getAffineX().toByteArray();
        int offset = 0;
        if (bin[0] == 0) offset = 1;
        System.arraycopy(bin, offset, out, 3 + 32 - bin.length + offset, bin.length - offset);

        bin = p.getAffineY().toByteArray();
        offset = 0;
        if (bin[0] == 0) offset = 1;
        System.arraycopy(bin, offset, out, out.length - bin.length + offset, bin.length - offset);
        return out;
    }

    @Override
    public KeyPair generateNewKeys() {
        try {
            KeyParameters keyParams = new KeyParameters();
            keyParams.setAllowDerive(true);
            keyParams.setAllowSign(false);
            KeyPairGenerator gen = KeyPairGenerator.getInstance("EC", "DYADIC");
            AlgorithmParameterSpec spec = new ECGenParameterSpec("secp256r1");
            spec = new KeyGenSpec(spec, keyParams);
            gen.initialize(spec);
            java.security.KeyPair keyPair = gen.generateKeyPair();
            ECPublicKey pub = (ECPublicKey) keyPair.getPublic();
            if (pub == null) {
                LOGGER.error("unable to retrieve a public key");
                throw new EncryptorException("unable to retrieve a public key");
            }

            byte[] pubEncoded = pub.getEncoded();

            byte[] der = ecPointToDer(pub.getW());
            byte[] hash = MessageDigest.getInstance("SHA-256").digest(der);

            return new KeyPair(PublicKey.from(pubEncoded), PrivateKey.from(Arrays.copyOf(hash, 8)));
        } catch (Exception e) {
            LOGGER.error("Unable to generate key pair");
            throw new EncryptorException("Unable to generate key pair: " + e.getMessage());
        }
    }

    static char hexChar(int x) {
        int hax = x & 0x0f;
        if (hax < 10) return (char) (hax + '0');
        return (char) (hax - 10 + 'a');
    }

    @Override
    public SharedKey computeSharedKey(PublicKey publicKey, PrivateKey privateKey) {
        try {
            char[] name = new char[20];
            name[0] = '0';
            name[1] = 'x';
            name[2] = '0';
            name[3] = '0';
            byte[] uid = privateKey.getKeyBytes();
            for (int i = 0; i < 8; i++) {
                name[4 + i * 2] = hexChar(uid[i] >> 4);
                name[4 + i * 2 + 1] = hexChar(uid[i]);
            }
            KeyStore keyStore = KeyStore.getInstance("PKCS11", "DYADIC");
            keyStore.load(null);
            java.security.PrivateKey prvKey = (java.security.PrivateKey) keyStore.getKey(new String(name), null);
            KeyAgreement keyAgree = KeyAgreement.getInstance("ECDH", "DYADIC");
            keyAgree.init(prvKey);

            KeyFactory kf = KeyFactory.getInstance("EC");
            LOGGER.info("Encode public key {}", publicKey.encodeToBase64());

            java.security.PublicKey pubKey = kf.generatePublic(new X509EncodedKeySpec(publicKey.getKeyBytes()));
            keyAgree.doPhase(pubKey, true);
            byte[] sharedSecret = keyAgree.generateSecret();
            byte[] hash = MessageDigest.getInstance("SHA-256").digest(sharedSecret);
            return SharedKey.from(hash);
        } catch (Exception e) {
            LOGGER.error("unable to generate shared secret", e);
            throw new EncryptorException("unable to generate shared secret: " + e.getMessage());
        }
    }

    @Override
    public byte[] sealAfterPrecomputation(byte[] message, Nonce nonce, SharedKey sharedKey) {
        try {
            Cipher cipher = Cipher.getInstance("AES/GCM/NoPadding", "SunJCE");
            cipher.init(
                Cipher.ENCRYPT_MODE,
                new SecretKeySpec(sharedKey.getKeyBytes(), "AES"),
                new GCMParameterSpec(AES_GCM_TAG_LEN * 8, nonce.getNonceBytes()));
            return cipher.doFinal(message);
        } catch (GeneralSecurityException e) {
            LOGGER.error("unable to perform symmetric encryption", e);
            throw new EncryptorException("unable to perform symmetric encryption: " + e.getMessage());
        }
    }

    @Override
    public byte[] openAfterPrecomputation(byte[] cipherText, Nonce nonce, SharedKey sharedKey) {
        try {
            Cipher cipher = Cipher.getInstance("AES/GCM/NoPadding", "SunJCE");
            cipher.init(
                Cipher.DECRYPT_MODE,
                new SecretKeySpec(sharedKey.getKeyBytes(), "AES"),
                new GCMParameterSpec(AES_GCM_TAG_LEN * 8, nonce.getNonceBytes()));
            return cipher.doFinal(cipherText);
        } catch (GeneralSecurityException e) {
            LOGGER.error("unable to perform symmetric decryption", e);
            throw new EncryptorException("unable to perform symmetric decryption: " + e.getMessage());
        }
    }

    @Override
    public byte[] seal(byte[] message, Nonce nonce, PublicKey publicKey, PrivateKey privateKey) {
        throw new UnsupportedOperationException();
    }

    @Override
    public byte[] open(byte[] cipherText, Nonce nonce, PublicKey publicKey, PrivateKey privateKey) {
        throw new UnsupportedOperationException();
    }

    @Override
    public Nonce randomNonce() {
        final byte[] nonceBytes = new byte[AES_GCM_IV_LEN];
        secureRandom.nextBytes(nonceBytes);
        return new Nonce(nonceBytes);
    }

    @Override
    public SharedKey createSingleKey() {
        final byte[] keyBytes = new byte[AES_KEY_LEN];
        secureRandom.nextBytes(keyBytes);
        return SharedKey.from(keyBytes);
    }
}
