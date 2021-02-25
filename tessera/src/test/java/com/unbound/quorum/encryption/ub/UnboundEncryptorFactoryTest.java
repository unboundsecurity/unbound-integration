package com.unbound.quorum.encryption.ub;

import org.junit.Before;
import org.junit.Test;

import static org.assertj.core.api.Assertions.assertThat;

import com.quorum.tessera.encryption.Encryptor;

public class UnboundEncryptorFactoryTest {
    private UnboundEncryptorFactory encryptorFactory;

    @Before
    public void setUp() {
        this.encryptorFactory = new UnboundEncryptorFactory();
    }

    @Test
    public void createInstance() {
        final Encryptor result = encryptorFactory.create();
        assertThat(encryptorFactory.getType()).isEqualTo("CUSTOM");
        assertThat(result).isNotNull().isExactlyInstanceOf(UnboundEncryptor.class);
    }
}
