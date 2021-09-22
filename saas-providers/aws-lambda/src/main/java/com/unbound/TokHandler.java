package com.unbound;

import com.unbound.provider.UBCryptoProvider;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.security.*;
import java.security.cert.CertificateException;

public interface TokHandler {

    static byte[] hexToBytes(String str) {
        int len = str.length();
        byte[] data = new byte[len / 2];
        for (int i = 0; i < len; i += 2) {
            data[i / 2] = (byte) ((Character.digit(str.charAt(i), 16) << 4)
                    + Character.digit(str.charAt(i + 1), 16));
        }
        return data;
    }

    static void writeBinaryFile(String filePath, byte[] binary) throws IOException {
        Path path = Paths.get(filePath);
        Files.write(path, binary);
    }

    default void loadUnbound() throws IOException, KeyStoreException, NoSuchProviderException, NoSuchAlgorithmException, CertificateException {
        byte[] p7b = hexToBytes(System.getenv("UKC_CA_DATA"));

        writeBinaryFile("/tmp/ca.p7b", p7b);
        System.setProperty("ukcCa", "/tmp/ca.p7b");

        Provider provider = new UBCryptoProvider();
        Security.addProvider(provider);
        KeyStore keyStore = KeyStore.getInstance("PKCS11", "UNBOUND");


        //if (user != null && password != null) {
        //    String auth = String.format("{\"username\":\"%s\", \"password\":\"%s\"}", user, password);
        //    keyStore.load(null, auth.toCharArray());
        //} else

        keyStore.load(null);

    }

}
