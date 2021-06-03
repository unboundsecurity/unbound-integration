package com.unbound.jmeter;

import com.dyadicsec.provider.DYCryptoProvider;
import com.unbound.provider.UBCryptoProvider;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.security.KeyStore;
import java.security.Security;
import java.util.ArrayList;
import java.util.Enumeration;

// singleton class that holds a list of keys name we have in the keystore
public class KeysHolder {

    private static final Logger log = LoggerFactory.getLogger(KeysHolder.class);

    private volatile static KeysHolder instance;
    protected static final String UNBOUND_PROVIDER_TYPE = "PKCS11";
    protected static final String UNBOUND_PROVIDER = "DYADIC";
    private static final String KEY_STORE_PASSWORD = "Password1!";

    private ArrayList<String> keysAliases = null;
    private static int instancesCounter = 0; // used to release the singleton object between reruns of test

    private KeysHolder() {
        UBCryptoProvider.allowPrivateKeyWithoutCertificate(true);
        DYCryptoProvider provider = new DYCryptoProvider();
        Security.addProvider(provider);

        try {
            KeyStore keyStore = KeyStore.getInstance(UNBOUND_PROVIDER_TYPE, UNBOUND_PROVIDER);
            keyStore.load(null, KEY_STORE_PASSWORD.toCharArray());

            Enumeration<String> enumeration = keyStore.aliases();
            keysAliases = new ArrayList();
            int i = 0;
            while (enumeration.hasMoreElements()) {
                String alias = enumeration.nextElement();
                keysAliases.add(i++, alias);
            }
            for (i = 0; i < keysAliases.size(); i++) {
                log.info(i + " alias: " + keysAliases.get(i));
            }
        } catch (Exception ex) {

            throw new RuntimeException(ex);
        }
    }

    public static synchronized KeysHolder getInstance() {
        if (instance == null) {
            instance = new KeysHolder();
        }
        instancesCounter++;
        return instance;
    }

    public static synchronized void releaseInstance(){
        if (--instancesCounter<=0){
            instance = null;
        }
    }

    public ArrayList<String> getKeysAliases(){
        return keysAliases;
    }
}
