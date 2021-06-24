// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    using System;
    using System.Security.Claims;
    using Microsoft.InformationProtection.Web.Models.Extensions;
    using sg = System.Globalization;
    using unbound.cryptoki;
    using System.Linq;
    using System.Text;

    public class KeyManager
    {
        private readonly IKeyStore keyStore;

        public KeyManager(IKeyStore keyStore)
        {
            this.keyStore = keyStore;
        }

        public KeyData GetPublicKey(Uri requestUri, string keyName)
        {
            //requestUri.ThrowIfNull(nameof(requestUri));
            keyName.ThrowIfNull(nameof(keyName));
            PublicKeyCache cache = null;
            ///////////////////////////////////////////////
            //use ukc as keystore
            byte[] keyNameBytes = Encoding.UTF8.GetBytes(keyName);
            //byte[] keyIDBytes = Encoding.UTF8.GetBytes("6a31bf60705cd8b9");
            //ulong answer = Convert.ToInt64("6a31bf60705cd8b9",16);
            //ulong keyUID = (ulong)Convert.ToUInt64("6a31bf60705cd8b9",16);

            Library.C_Initialize();
            CK_SLOT_ID[] slots = Library.C_GetSlotList(true);
            CK_SLOT_ID slot = slots[0];
            CK_SESSION_HANDLE session = Library.C_OpenSession(slot);

            Library.C_FindObjectsInit(session, new CK_ATTRIBUTE[]
            {
                new CK_ATTRIBUTE(CK.CKA_TOKEN, true),
                new CK_ATTRIBUTE(CK.CKA_CLASS, CK.CKO_PRIVATE_KEY),
                new CK_ATTRIBUTE(CK.CKA_KEY_TYPE, CK.CKK_RSA),
                new CK_ATTRIBUTE(CK.CKA_ID, keyNameBytes),
                //new CK_ATTRIBUTE(CK.DYCKA_UID , keyUID)
            });

            CK_OBJECT_HANDLE[] foundKeyHandles = Library.C_FindObjects(session, 1);
            Library.C_FindObjectsFinal(session);

            CK_ATTRIBUTE n =  new CK_ATTRIBUTE(CK.CKA_MODULUS);
            CK_ATTRIBUTE e = new CK_ATTRIBUTE(CK.CKA_PUBLIC_EXPONENT);
            CK_ATTRIBUTE privateKeyUid =  new CK_ATTRIBUTE(CK.DYCKA_UID); 

            if(foundKeyHandles.Length > 0)
            {

                //get public key
                Library.C_GetAttributeValue(session, foundKeyHandles[0],new CK_ATTRIBUTE[]
                {
                    n,
                    e,
                    privateKeyUid
                });

                string nStrBase64 = Convert.ToBase64String((byte[])n.pValue);
                //string uid = (UInt64)privateKeyUid.pValue;
                //string vOut = (UInt64)privateKeyUid.pValue;


                //build the public key obj
                var publicKeyFromUkc = new PublicKey(nStrBase64,65537);
                
                //publicKeyFromUkc.KeyId = ((UInt64)privateKeyUid.pValue).ToString();
                publicKeyFromUkc.KeyType = "RSA";
                publicKeyFromUkc.Algorithm = "RS256";
                
                return new KeyData(publicKeyFromUkc, cache);


            }


            //////////////////////////////////////////////
            //TODO: should remove this(microsoft code)
            var key = keyStore.GetActiveKey(keyName);
            var publicKey = key.Key.GetPublicKey();

            publicKey.KeyId = requestUri.GetLeftPart(UriPartial.Path) + "/" + key.KeyId;
            publicKey.KeyType = key.KeyType;
            publicKey.Algorithm = key.SupportedAlgorithm;

            if(key.ExpirationTimeInDays.HasValue)
            {
                cache = new PublicKeyCache(
                    DateTime.UtcNow.AddDays(
                        key.ExpirationTimeInDays.Value).ToString("yyyy-MM-ddTHH:mm:ss", sg.CultureInfo.InvariantCulture));
            }

            return new KeyData(publicKey, cache);
            ////////////////////////////////////////////////
        }

        public DecryptedData Decrypt(ClaimsPrincipal user, string keyName, string keyId, EncryptedData encryptedData)
        {
            user.ThrowIfNull(nameof(user));
            keyName.ThrowIfNull(nameof(keyName));
            keyId.ThrowIfNull(nameof(keyId));
            encryptedData.ThrowIfNull(nameof(encryptedData));

            //var keyData = keyStore.GetKey(keyName, keyId);
            //keyData.KeyAuth.CanUserAccessKey(user, keyData);


            /////////////////////////////////////////
            //get key data from ukc
             ///////////////////////////////////////////////
            //keyName="key1";
            //use ukc as keystore
            byte[] keyNameBytes = Encoding.UTF8.GetBytes(keyName);
            ulong keyUID = (ulong)Convert.ToUInt64(keyId,16);

             CK_OBJECT_HANDLE pubKey;
             CK_OBJECT_HANDLE prvKey;
             CK_OBJECT_HANDLE publicTest;


            Library.C_Initialize();
            CK_SLOT_ID[] slots = Library.C_GetSlotList(true);
            CK_SLOT_ID slot = slots[0];
            CK_SESSION_HANDLE session = Library.C_OpenSession(slot);

            Library.C_FindObjectsInit(session, new CK_ATTRIBUTE[]
            {
                new CK_ATTRIBUTE(CK.CKA_TOKEN, true),
                new CK_ATTRIBUTE(CK.CKA_CLASS, CK.CKO_PRIVATE_KEY),
                new CK_ATTRIBUTE(CK.CKA_KEY_TYPE, CK.CKK_RSA),
                //new CK_ATTRIBUTE(CK.CKA_ID, keyNameBytes),
                new CK_ATTRIBUTE(CK.DYCKA_UID , keyUID)

            });

            CK_OBJECT_HANDLE[] foundKeyHandles = Library.C_FindObjects(session, 1);
            Library.C_FindObjectsFinal(session);

            CK_ATTRIBUTE n =  new CK_ATTRIBUTE(CK.CKA_MODULUS);
            CK_ATTRIBUTE e = new CK_ATTRIBUTE(CK.CKA_PUBLIC_EXPONENT);
            CK_ATTRIBUTE privateKeyUid =  new CK_ATTRIBUTE(CK.DYCKA_UID); 

            if(foundKeyHandles.Length > 0)
            {

                //get public key
                Library.C_GetAttributeValue(session, foundKeyHandles[0],new CK_ATTRIBUTE[]
                {
                    n,
                    e,
                    privateKeyUid
                });

                string nStrBase64 = Convert.ToBase64String((byte[])n.pValue);
                //string uid = (UInt64)privateKeyUid.pValue;
                //string vOut = (UInt64)privateKeyUid.pValue;

                 publicTest = Library.C_CreateObject(session,new CK_ATTRIBUTE[]
                {
                    new CK_ATTRIBUTE(CK.CKA_TOKEN, false),
                    new CK_ATTRIBUTE(CK.CKA_CLASS, CK.CKO_PUBLIC_KEY),
                    new CK_ATTRIBUTE(CK.CKA_KEY_TYPE, CK.CKK_RSA),
                    n,
                    e
                });

                //build the public key obj
                var publicKeyFromUkc = new PublicKey(nStrBase64,65537);
                publicKeyFromUkc.KeyType = "RSA";
                publicKeyFromUkc.Algorithm = "RS256";

                //CK_OBJECT_HANDLE hKey = new CK_OBJECT_HANDLE(vOut);    

                byte[] plainData = Encoding.UTF8.GetBytes(encryptedData.Value);

                    Console.WriteLine("Set RSA padding params");
                    CK_RSA_PKCS_OAEP_PARAMS oaepParams = new CK_RSA_PKCS_OAEP_PARAMS();
                    oaepParams.hashAlg = CK.CKM_SHA256;
                    oaepParams.mgf = CK.CKG_MGF1_SHA256;
                    CK_MECHANISM mech_rsa = new CK_MECHANISM(CK.CKM_RSA_PKCS_OAEP, oaepParams);
                      // Encrypt Data
                    //System.out.println("Encrypt Data");
                    
                    Library.C_EncryptInit(session, mech_rsa, publicTest);
                    byte[] encrypted = Library.C_Encrypt(session, plainData);

                    // Decrypt Data
                    //System.out.println("Decrypt Data");
                    Library.C_DecryptInit(session, mech_rsa, foundKeyHandles[0]);
                    byte[] decrypted = Library.C_Decrypt(session, encrypted);


                    if (!Enumerable.SequenceEqual(plainData, decrypted)) throw new Exception("ENC/DEC mismatch");
                    ASCIIEncoding ByteConverter = new ASCIIEncoding();

                    return new DecryptedData(ByteConverter.GetString(decrypted));



            }


            //////////////////////////////////////////

            //keyData.KeyAuth.CanUserAccessKey(user, keyData);

            // if (encryptedData.Algorithm != "RSA-OAEP-256")
            // {
            //     throw new ArgumentException(encryptedData.Algorithm + " is not supported");
            // }

            //Create a UnicodeEncoder to convert between byte array and string.
             //ASCIIEncoding ByteConverter = new ASCIIEncoding();

            // string dataString = encryptedData.Value;

            // //Create byte arrays to hold original, encrypted, and decrypted data.
            // byte[] dataToEncrypt = ByteConverter.GetBytes(dataString);
            // byte[] encryptedData2;
            // byte[] decryptedData2;

            // //Create a new instance of the RSACryptoServiceProvider class
            // // and automatically create a new key-pair.
            // RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

            // //Display the origianl data to the console.
            // Console.WriteLine("Original Data: {0}", dataString);

            // //Encrypt the byte array and specify no OAEP padding.
            // //OAEP padding is only available on Microsoft Windows XP or
            // //later.
            // encryptedData2 = RSAalg.Encrypt(dataToEncrypt, false);

            // //Display the encrypted data to the console.
            // Console.WriteLine("Encrypted Data: {0}", ByteConverter.GetString(encryptedData2));

            // //Pass the data to ENCRYPT and boolean flag specifying
            // //no OAEP padding.
            // decryptedData2 = RSAalg.Decrypt(encryptedData2, false);

            // //Display the decrypted plaintext to the console.
            // Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData2));
            

            // //try encrpt with ukc
            // Library.C_Initialize();
            // CK_SLOT_ID[] slots = Library.C_GetSlotList(true);
            // CK_SLOT_ID slot = slots[0];
            // CK_SESSION_HANDLE session = Library.C_OpenSession(slot);
            // CK_OBJECT_HANDLE pubKey;
            // CK_OBJECT_HANDLE prvKey;
            // byte[] keyName2 = Encoding.UTF8.GetBytes("key1");


            //   // Generate key pair
            //     //System.out.println("Generate key pair");
            //     Library.C_GenerateKeyPair(session,
            //     new CK_MECHANISM(CK.CKM_RSA_PKCS_KEY_PAIR_GEN), // RSA generation mechanism
            //     new CK_ATTRIBUTE[]
            //     {
            //         new CK_ATTRIBUTE(CK.CKA_TOKEN, false),
            //         new CK_ATTRIBUTE(CK.CKA_MODULUS_BITS, 2048), // RSA key size
            //     },
            //     new CK_ATTRIBUTE[]
            //     {
            //         new CK_ATTRIBUTE(CK.CKA_TOKEN, true),
            //         new CK_ATTRIBUTE(CK.CKA_ID, keyName2),
            //     },out pubKey,out prvKey);
            //     //int pubKey = keyHandles[0];
            //     //int prvKey = keyHandles[1];



            // byte[] plainData = Encoding.UTF8.GetBytes("TEST PLAIN DATA");

            // Console.WriteLine("Set RSA padding params");
            //     CK_RSA_PKCS_OAEP_PARAMS oaepParams = new CK_RSA_PKCS_OAEP_PARAMS();
            //     oaepParams.hashAlg = CK.CKM_SHA256;
            //     oaepParams.mgf = CK.CKG_MGF1_SHA256;
            //     CK_MECHANISM mech_rsa = new CK_MECHANISM(CK.CKM_RSA_PKCS_OAEP, oaepParams);
            //       // Encrypt Data
            //     //System.out.println("Encrypt Data");
                
            //     Library.C_EncryptInit(session, mech_rsa, pubKey);
            //     byte[] encrypted = Library.C_Encrypt(session, plainData);

            //var testEncryptedValue = "CthOUMzRdtSwo+4twgtjCA674G3UosWypUZv5E7uxG7GqYPiIJ+E+Uq7vbElp/bahB1fJrgq1qbdMrUZnSypVqBwYnccSxwablO15OOXl9Rn1e7w9V9fuMxtUqvhn+YZezk1623Qd7f5XTYjf6POwixtrgfZtdA+qh00ktKiVBpQKNG/bxhV94fK9+hb+qnzPmXilr9QF5rSQTd4hYHmYcR2ljVCDDZMV3tCVUTecWjS5HbOA1254ve/q3ulBLoPQTE58g7FwDQUZnd7XBdRSwYnrBWTJh8nmJ0PDfn+mCTGEI86S7HtoFYsE+Hezd24Z523phGEVrdMC9Ob1LlXEA==";

            //var decryptedData = keyData.Key.Decrypt(Convert.FromBase64String(encryptedData.Value));
            throw new Exception("Faild to decrypt");
            //return new DecryptedData("Faild to decrypt");
        }
    }
}