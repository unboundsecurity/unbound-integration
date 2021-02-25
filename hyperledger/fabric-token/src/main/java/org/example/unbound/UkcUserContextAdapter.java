package org.example.unbound;

import com.dyadicsec.provider.DYCryptoProvider;
import org.example.user.CAEnrollment;
import org.example.user.UserContext;
import org.hyperledger.fabric.sdk.Enrollment;

import java.io.*;
import java.security.GeneralSecurityException;
import java.security.KeyStore;
import java.security.PrivateKey;
import java.security.Security;
import java.security.cert.CertificateException;
import java.security.cert.CertificateFactory;
import java.security.cert.X509Certificate;
import java.security.interfaces.ECPrivateKey;
import java.util.Base64;

public class UkcUserContextAdapter {

    private static final String DYADIC_PROVIDER_TYPE = "PKCS11";
    private static final String DYADIC_PROVIDER_NAME = "DYADIC";

    /**
     *
     * @param args
     * @throws IOException
     * @throws ClassNotFoundException
     * @throws GeneralSecurityException
     */
    public static void main(String[] args) throws IOException, ClassNotFoundException, GeneralSecurityException {
        String filePath = args[0];

        File file = new File(filePath);
        if (file.exists()) {
            // Reading the object from a file
            FileInputStream fileStream = new FileInputStream(filePath);
            ObjectInputStream in = new ObjectInputStream(fileStream);

            // Method for deserialization of object
            UserContext uContext = (UserContext) in.readObject();
            adapt(uContext,false);

            in.close();
            fileStream.close();
        } else {
            throw new FileNotFoundException(filePath);
        }
    }

    /**
     *
     * @param pem
     * @return
     * @throws CertificateException
     */
    public static X509Certificate pemToCert(String pem) throws CertificateException {

        byte[] der = Base64.getDecoder().decode(
                pem
                        .replace("-----BEGIN CERTIFICATE-----", "")
                        .replace("-----END CERTIFICATE-----", "")
                        .replace("\n", "")
                        .replace("\r", ""));

        CertificateFactory certificateFactory = CertificateFactory.getInstance("X.509");
        return (X509Certificate) certificateFactory.generateCertificate(new ByteArrayInputStream(der));
    }

    /**
     * @param uContext
     * @throws GeneralSecurityException
     * @throws IOException
     */
    public static void adapt(UserContext uContext, boolean keepPrivate) throws GeneralSecurityException, IOException {

        PrivateKey privateKey = uContext.getEnrollment().getKey();
        String certificate = uContext.getEnrollment().getCert();

        X509Certificate x509Certificate = pemToCert(certificate);

        Security.addProvider(new DYCryptoProvider());

        KeyStore keyStore = KeyStore.getInstance(DYADIC_PROVIDER_TYPE, DYADIC_PROVIDER_NAME);
        keyStore.load(null);

        String keyAlias = keyStore.getCertificateAlias(x509Certificate);

        ECPrivateKey ecPrivateKey = null;

        if (keyAlias == null) {
            keyStore.setEntry(uContext.getName(), new DYCryptoProvider.KeyEntry(privateKey), null);
            keyStore.setCertificateEntry(uContext.getName(), x509Certificate);
        }

        ecPrivateKey = (ECPrivateKey) keyStore.getKey(uContext.getName(), null);
        uContext.setEnrollment(new CAEnrollment(keepPrivate ? ecPrivateKey : null,certificate));
    }
}
