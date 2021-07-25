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
        private readonly ILogger _logger;

        public KeyManager(ILogger<KeyManager> logger)
        {
             _logger = logger;
        }

        public KeyData GetPublicKey(Uri requestUri, string keyName)
        {
            _logger.LogInformation("get public key : " + keyName );

            //requestUri.ThrowIfNull(nameof(requestUri));
            keyName.ThrowIfNull(nameof(keyName));
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

            if(foundKeyHandles.Length == 0) throw new Exception("key" + keyName + " not found");
            
            //get public key
            Library.C_GetAttributeValue(session, foundKeyHandles[0],new CK_ATTRIBUTE[]
            {
                n,
                e,
                privateKeyUid
            });

            string nStrBase64 = Convert.ToBase64String((byte[])n.pValue);
            var KeyId = Convert.ToString((long)privateKeyUid.pValue,16);
            var publicKey = new PublicKey(nStrBase64,65537);
            string websiteHostName = System.Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            publicKey.KeyId = "https://" + websiteHostName + "/" + KeyId;
            publicKey.KeyType = "RSA";
            publicKey.Algorithm = "RS256";

            return new KeyData(publicKey);
              
        }

        public DecryptedData Decrypt(ClaimsPrincipal user, string keyName, string keyId, EncryptedData encryptedData)
        {
            _logger.LogInformation("decrypt called from key manager class for keyName : " + keyName + " and keyID : " + keyId );
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

            if(foundKeyHandles.Length == 0) throw new Exception("key" + keyName + " not found");

            _logger.LogInformation("encryptedData.Value = "  + encryptedData.Value);

            byte[] plainData = Convert.FromBase64String(encryptedData.Value);

            CK_RSA_PKCS_OAEP_PARAMS oaepParams = new CK_RSA_PKCS_OAEP_PARAMS();
            oaepParams.hashAlg = CK.CKM_SHA256;
            oaepParams.mgf = CK.CKG_MGF1_SHA256;
            CK_MECHANISM mech_rsa = new CK_MECHANISM(CK.CKM_RSA_PKCS_OAEP, oaepParams);
            
            Library.C_DecryptInit(session, mech_rsa, foundKeyHandles[0]);
            byte[] decrypted = Library.C_Decrypt(session, plainData);

            return new DecryptedData(Convert.ToBase64String(decrypted));        
        }
    }
}