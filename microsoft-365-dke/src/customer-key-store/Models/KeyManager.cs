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
    using Microsoft.Extensions.Logging;


    public class KeyManager
    {
        private readonly IKeyStore keyStore;
        private readonly ILogger _logger;


        public KeyManager(IKeyStore keyStore,ILogger<KeyManager> logger)
        {
            this.keyStore = keyStore;
             _logger = logger;
        }

        public KeyData GetPublicKey(Uri requestUri, string keyName)
        {
            _logger.LogInformation("get public key : " + keyName );
            //requestUri.ThrowIfNull(nameof(requestUri));
            keyName.ThrowIfNull(nameof(keyName));
            PublicKeyCache cache = null;
            //use ukc to search the key
            byte[] keyNameBytes = Encoding.UTF8.GetBytes(keyName);

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
                var key = keyStore.GetActiveKey(keyName);
                var publicKey = new PublicKey(nStrBase64,65537);

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

            }
        }

        public DecryptedData Decrypt(ClaimsPrincipal user, string keyName, string keyId, EncryptedData encryptedData)
        {
            _logger.LogInformation("decrypt called from key manager class for keyName : " + keyName + " and keyID : " + keyId );
            Console.WriteLine("decrypt called");
            user.ThrowIfNull(nameof(user));
            keyName.ThrowIfNull(nameof(keyName));
            keyId.ThrowIfNull(nameof(keyId));
            encryptedData.ThrowIfNull(nameof(encryptedData));
            //use ukc to decrypt
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
                _logger.LogInformation("encryptedData.Value = "  + encryptedData.Value);


                //byte[] plainData = Encoding.UTF8.GetBytes(encryptedData.Value);
                byte[] plainData = Convert.FromBase64String(encryptedData.Value);

                Console.WriteLine("Set RSA padding params");
                CK_RSA_PKCS_OAEP_PARAMS oaepParams = new CK_RSA_PKCS_OAEP_PARAMS();
                oaepParams.hashAlg = CK.CKM_SHA256;
                oaepParams.mgf = CK.CKG_MGF1_SHA256;
                CK_MECHANISM mech_rsa = new CK_MECHANISM(CK.CKM_RSA_PKCS_OAEP, oaepParams);
                // Encrypt Data
                //System.out.println("Encrypt Data");
                
                //Library.C_EncryptInit(session, mech_rsa, publicTest);
                //byte[] encrypted = Library.C_Encrypt(session, plainData);

                // Decrypt Data
                //System.out.println("Decrypt Data");
                Library.C_DecryptInit(session, mech_rsa, foundKeyHandles[0]);
                byte[] decrypted = Library.C_Decrypt(session, plainData);

                //if (!Enumerable.SequenceEqual(plainData, decrypted)) throw new Exception("ENC/DEC mismatch");
                //ASCIIEncoding ByteConverter = new ASCIIEncoding();

                return new DecryptedData(Convert.ToBase64String(decrypted));
            }

            _logger.LogInformation("Faild to decrypt");
            throw new Exception("Faild to decrypt");
        }
    }
}