// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    using System;
    using Microsoft.InformationProtection.Web.Models.Extensions;
    using sg = System.Globalization;
    using System.Security.Cryptography;
    using System.Text;


    public class TestKey : IKey
    {
        private string privateKeyPem;
        private string publicKeyPem;
        private PublicKey storedPublicKey = null;
        private System.Security.Cryptography.RSA cryptoEngine = null;

        public TestKey(string publicKey, string privateKey)
        {
            publicKeyPem = publicKey;
            privateKeyPem = privateKey;
        }

        public PublicKey GetPublicKey()
        {
            IntializeCrypto();

            return storedPublicKey;
        }

        //TODO: should remove this function
        public byte[] Decrypt(byte[] encryptedData)
        {
             //Create a UnicodeEncoder to convert between byte array and string.
            ASCIIEncoding ByteConverter = new ASCIIEncoding();

            //string dataString = "michael";

            //Create byte arrays to hold original, encrypted, and decrypted data.
            byte[] dataToEncrypt = encryptedData;
            byte[] encryptedData2;
            byte[] decryptedData2;
            
            IntializeCrypto();
            RSAParameters rsaParams = getRSAParameters();
            //Create a new instance of the RSACryptoServiceProvider class
            // and automatically create a new key-pair.
            RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
            //Import key parameters into RSA.
            //RSAalg.ImportParameters(rsaParams);

            //Display the origianl data to the console.
            //Console.WriteLine("Original Data: {0}", dataString);

            //Encrypt the byte array and specify no OAEP padding.
            //OAEP padding is only available on Microsoft Windows XP or
            //later.
            encryptedData2 = RSAalg.Encrypt(dataToEncrypt, false);

            //Display the encrypted data to the console.
            Console.WriteLine("Encrypted Data: {0}", ByteConverter.GetString(encryptedData2));

            //Pass the data to ENCRYPT and boolean flag specifying
            //no OAEP padding.
            decryptedData2 = RSAalg.Decrypt(encryptedData2, false);

            //Display the decrypted plaintext to the console.
            Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData2));

            return decryptedData2;

            //return cryptoEngine.Decrypt(encryptedData, System.Security.Cryptography.RSAEncryptionPadding.OaepSHA256);
        }

        private static uint ByteArrayToUInt(byte[] array)
        {
            uint retVal = 0;

            checked
            {
              if (BitConverter.IsLittleEndian)
              {
                  for (int index = array.Length - 1; index >= 0; index--)
                  {
                      retVal = (retVal << 8) + array[index];
                  }
              }
              else
              {
                  for (int index = 0; index < array.Length; index++)
                  {
                      retVal = (retVal << 8) + array[index];
                  }
              }
            }

            return retVal;
        }

        private void IntializeCrypto()
        {
            if(cryptoEngine == null)
            {
                var tempCryptoEngine = System.Security.Cryptography.RSA.Create();
                byte[] privateKeyBytes = System.Convert.FromBase64String(privateKeyPem);
                tempCryptoEngine.ImportRSAPrivateKey(privateKeyBytes, out int bytesRead);

                var rsaKeyInfo = tempCryptoEngine.ExportParameters(false);
                var exponent = ByteArrayToUInt(rsaKeyInfo.Exponent);
                var modulus = Convert.ToBase64String(rsaKeyInfo.Modulus);
                storedPublicKey = new PublicKey(modulus, exponent);

                cryptoEngine = tempCryptoEngine;
            }
        }

        //TODO: remove this function
        private RSAParameters getRSAParameters()
        {
        
         //Create a new instance of RSAParameters.
            RSAParameters RSAKeyInfo = new RSAParameters();

                var tempCryptoEngine = System.Security.Cryptography.RSA.Create();
                byte[] privateKeyBytes = System.Convert.FromBase64String(privateKeyPem);
                tempCryptoEngine.ImportRSAPrivateKey(privateKeyBytes, out int bytesRead);

                var rsaKeyInfo = tempCryptoEngine.ExportParameters(false);
                var exponent = ByteArrayToUInt(rsaKeyInfo.Exponent);
                var modulus = Convert.ToBase64String(rsaKeyInfo.Modulus);
                storedPublicKey = new PublicKey(modulus, exponent);

                //cryptoEngine = tempCryptoEngine;

                    //Set RSAKeyInfo to the public key values. 
                RSAKeyInfo.Modulus = rsaKeyInfo.Modulus;
                RSAKeyInfo.Exponent = rsaKeyInfo.Exponent;

                return RSAKeyInfo;
            
        }
    }
}