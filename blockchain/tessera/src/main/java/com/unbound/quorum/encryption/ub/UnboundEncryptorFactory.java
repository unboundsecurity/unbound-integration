package com.unbound.quorum.encryption.ub;



import com.dyadicsec.provider.DYCryptoProvider;
import com.quorum.tessera.encryption.Encryptor;
import com.quorum.tessera.encryption.EncryptorFactory;

import java.util.Map;

public class UnboundEncryptorFactory implements EncryptorFactory {

    @Override
    public String getType() {
        return "CUSTOM";
    }

    @Override
    public Encryptor create(Map<String, String> properties) {
        DYCryptoProvider.allowPrivateKeyWithoutCertificate(true);
        return new UnboundEncryptor();
    }
}
