package com.unbound.quorum.encryption.ub;

import com.quorum.tessera.encryption.*;
import org.junit.Ignore;
import org.junit.Test;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.util.Base64;


import static org.assertj.core.api.Assertions.assertThat;
import static org.junit.Assert.assertEquals;

public class UnboundEncryptorTest {
    private static final Logger LOGGER = LoggerFactory.getLogger(UnboundEncryptor.class);
    private final EncryptorFactory facadeFactory = new UnboundEncryptorFactory();

    private final UnboundEncryptor encryptor = (UnboundEncryptor) facadeFactory.create();

    @Ignore("Remove Ignore tag if UKC is running, test should pass if UKC is running")
    @Test()
    public void computeSharedKey() {
        KeyPair keyPair1 = encryptor.generateNewKeys();
        KeyPair keyPair2 = encryptor.generateNewKeys();
        SharedKey sharedPub1Priv2 = encryptor.computeSharedKey(keyPair1.getPublicKey(), keyPair2.getPrivateKey());
        SharedKey sharedPriv1Pub2 = encryptor.computeSharedKey(keyPair2.getPublicKey(), keyPair1.getPrivateKey());
        assertEquals(sharedPub1Priv2, sharedPriv1Pub2);
        LOGGER.info("SharedKey: {}", sharedPriv1Pub2.encodeToBase64());
    }
    @Test(expected = RuntimeException.class)
    public void computeSharedKeyWithInvalidKeys() {
        encryptor.computeSharedKey(PublicKey.from("garbage".getBytes()), PrivateKey.from("garbage".getBytes()));
    }

    @Test(expected = UnsupportedOperationException.class)
    public void seal() {
        encryptor.seal(null, null, null, null);
    }

    @Test(expected = UnsupportedOperationException.class)
    public void open() {
        encryptor.open(null, null, null, null);
    }

    @Test
    public void sealOpenAfterPrecomputation() {
        MasterKey masterKey = encryptor.createMasterKey();
        byte[] clearText = "MessageToEncrypt123".getBytes();
        Nonce nonce = encryptor.randomNonce();
        byte[] cipherText = encryptor.sealAfterPrecomputation(clearText, nonce, masterKey);
        LOGGER.info("Encrypted outpout: {}", Base64.getEncoder().encode(cipherText));
        byte[] decryptedText = encryptor.openAfterPrecomputation(cipherText, nonce, masterKey);
        assertThat(decryptedText).containsExactly(clearText);
    }

    @Test(expected = EncryptorException.class)
    public void openAfterPrecomputationInvalidSymmetricCipher() {
        UnboundEncryptor facade = new UnboundEncryptor();
        MasterKey masterKey = encryptor.createMasterKey();
        Nonce nonce = encryptor.randomNonce();
        facade.openAfterPrecomputation("test".getBytes(), nonce, masterKey);
    }

    @Test
    public void randomNonce() {
        Nonce nonce = encryptor.randomNonce();
        assertThat(nonce).isNotNull();
        assertThat(nonce.getNonceBytes()).hasSize(16);
    }

    @Ignore("Remove Ignore tag if UKC is running, test should pass if UKC is running")
    @Test
    public void generateNewKeys() {
        KeyPair keyPair = encryptor.generateNewKeys();
        assertThat(keyPair).isNotNull();
        assertThat(keyPair.getPublicKey()).isNotNull();
        assertThat(keyPair.getPrivateKey()).isNotNull();

        LOGGER.info("Public key size: {}", keyPair.getPublicKey().getKeyBytes().length);
        LOGGER.info("Private key size: {}", keyPair.getPrivateKey().getKeyBytes().length);
        String b64encodedPrivateKey = Base64.getEncoder().encodeToString(keyPair.getPrivateKey().getKeyBytes());
        LOGGER.info("base64 encoded private key: {}", b64encodedPrivateKey);
        LOGGER.info("base64 encoded private key length: {}", b64encodedPrivateKey.length());
        String b64encodedPublicKey = Base64.getEncoder().encodeToString(keyPair.getPublicKey().getKeyBytes());
        LOGGER.info("base64 encoded public key: {}", b64encodedPublicKey);
        LOGGER.info("base64 encoded public key length: {}", b64encodedPublicKey.length());
    }

    @Test
    public void createSingleKey() {
        SharedKey sharedKey = encryptor.createSingleKey();
        assertThat(sharedKey).isNotNull();
        assertThat(sharedKey.getKeyBytes()).hasSize(16);
    }

}
