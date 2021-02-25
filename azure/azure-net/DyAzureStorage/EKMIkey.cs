using Microsoft.Azure.KeyVault.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlobGettingStarted
{
    class EKMIkey : IKey
    {

        private string keyName;
        RSACng rsaPrivateKey; 

        public EKMIkey(string name)
        {
            keyName = name;
            CngProvider provider = new CngProvider("Dyadic Security Key Storage Provider");
            rsaPrivateKey = new RSACng(CngKey.Open(name, provider));
        }

        public string DefaultEncryptionAlgorithm 
        { 
            get 
            {
                // RSA using Optimal Asymmetric Encryption Padding (OAEP)	
                // RSA-OAEP	
                // http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p	
                // RSA/ECB/OAEPWithSHA-1AndMGF1Padding
                return "RSA-OAEP"; 
            } 
        }

        public string DefaultKeyWrapAlgorithm 
        { 
            get 
            {
                // RSA using Optimal Asymmetric Encryption Padding (OAEP)	
                // RSA-OAEP	
                // http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p	
                // RSA/ECB/OAEPWithSHA-1AndMGF1Padding
                return "RSA-OAEP";
            } 
        }

        public string DefaultSignatureAlgorithm 
        { 
            get 
            {
                // RSA using SHA-256 hash algorithm	RS256	
                // http://www.w3.org/2001/04/xmldsig-more#rsa-sha256	
                // SHA256withRSA	1.2.840.113549.1.1.11
                return "RS256"; 
            } 
        }

        public string Kid 
        { 
            get 
            { 
                return keyName; 
            } 
        }

        public System.Threading.Tasks.Task<Tuple<byte[], byte[], string>> 
            EncryptAsync(byte[] plaintext, byte[] iv, byte[] authenticationData, string algorithm, System.Threading.CancellationToken token)
        {
            byte[] cipher = rsaPrivateKey.Encrypt(plaintext, RSAEncryptionPadding.OaepSHA1);
            Tuple<byte[], byte[], string> result = Tuple.Create(cipher, (byte[])null, DefaultEncryptionAlgorithm);
            return Task.FromResult(result);
        }

        public System.Threading.Tasks.Task<byte[]> 
            DecryptAsync(byte[] ciphertext, byte[] iv, byte[] authenticationData, byte[] authenticationTag, string algorithm, System.Threading.CancellationToken token)
        {
            byte[] plaintext = rsaPrivateKey.Decrypt(ciphertext, RSAEncryptionPadding.OaepSHA1);
            return Task.FromResult(plaintext);
        }
        
        public System.Threading.Tasks.Task<Tuple<byte[], string>> 
            SignAsync(byte[] digest, string algorithm, System.Threading.CancellationToken token)
        {
            byte[] cipher = rsaPrivateKey.SignHash(digest, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            Tuple<byte[], string> result = Tuple.Create(cipher, DefaultSignatureAlgorithm);
            return Task.FromResult(result);
        }

        public System.Threading.Tasks.Task<bool> 
            VerifyAsync(byte[] digest, byte[] signature, string algorithm, System.Threading.CancellationToken token)
        {
            bool result = rsaPrivateKey.VerifyHash(digest, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Task.FromResult(result);
        }

        public System.Threading.Tasks.Task<byte[]> 
            UnwrapKeyAsync(byte[] encryptedKey, string algorithm, System.Threading.CancellationToken token)
        {
            return DecryptAsync(encryptedKey, null, null, null, algorithm, token);
        }

        public System.Threading.Tasks.Task<Tuple<byte[], string>> 
            WrapKeyAsync(byte[] key, string algorithm, System.Threading.CancellationToken token)
        {
            byte[] cipher = rsaPrivateKey.Encrypt(key, RSAEncryptionPadding.OaepSHA1);
            Tuple<byte[], string> result = Tuple.Create(cipher, DefaultKeyWrapAlgorithm);
            return Task.FromResult(result);
        }

        public void Dispose()
        {

        }
    }
}
