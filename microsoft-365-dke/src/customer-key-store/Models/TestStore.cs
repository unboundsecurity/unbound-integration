// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    using Microsoft.InformationProtection.Web.Models.Extensions;
    using sg = System.Globalization;
    using Microsoft.Extensions.Logging;
    using unbound.cryptoki;
    using System.Text;


    public class TestKeyStore : IKeyStore
    {
        private readonly ILogger _logger;
        private const string KeyType = "RSA";
        private const string Algorithm = "RS256";
        private Dictionary<string, Dictionary<string, KeyStoreData>> keys = new Dictionary<string, Dictionary<string, KeyStoreData>>();
        private Dictionary<string, string> activeKeys = new Dictionary<string, string>();

        public TestKeyStore(IConfiguration configuration,ILogger<TestKeyStore> logger)
        {
             _logger = logger;
            configuration.ThrowIfNull(nameof(configuration));

            var testKeysSection = configuration.GetSection("TestKeys");
            IAuthorizer keyAuth = null;

            if(!testKeysSection.Exists())
            {
                throw new System.ArgumentException("TestKeys section does not exist");
            }

            foreach(var testKey in testKeysSection.GetChildren())
            {
                List<string> roles = new List<string>();
                var validRoles = testKey.GetSection("AuthorizedRoles");
                var validEmails = testKey.GetSection("AuthorizedEmailAddress");

                if(validRoles != null && validRoles.Exists() &&
                   validEmails != null && validEmails.Exists())
                {
                    throw new System.ArgumentException("Both role and email authorizers cannot be used on the same test key");
                }

                if(validRoles != null && validRoles.Exists())
                {
                    RoleAuthorizer roleAuth = new RoleAuthorizer(configuration);
                    keyAuth = roleAuth;
                    foreach(var role in validRoles.GetChildren())
                    {
                        roleAuth.AddRole(role.Value);
                    }
                }
                else if(validEmails != null && validEmails.Exists())
                {
                    EmailAuthorizer emailAuth = new EmailAuthorizer();
                    keyAuth = emailAuth;
                    foreach(var email in validEmails.GetChildren())
                    {
                        emailAuth.AddEmail(email.Value);
                    }
                }

                int? expirationTimeInDays = null;
                var cacheTime = testKey["CacheExpirationInDays"];
                if(cacheTime != null)
                {
                    expirationTimeInDays = Convert.ToInt32(cacheTime, sg.CultureInfo.InvariantCulture);
                }

                var name = testKey["Name"];
                var id = testKey["Id"];
                var publicPem = testKey["PublicPem"];
                var privatePem = testKey["PrivatePem"];

                if(name == null)
                {
                  throw new System.ArgumentException("The key must have a name");
                }

                if(id == null)
                {
                  throw new System.ArgumentException("The key must have an id");
                }

                if(publicPem == null)
                {
                  throw new System.ArgumentException("The key must have a publicPem");
                }

                if(privatePem == null)
                {
                  throw new System.ArgumentException("The key must have a privatePem");
                }
                _logger.LogInformation("testStore constructe will createTestKey with following params:name=" + name +" and id=" + id);

                  CreateTestKey(
                    name,
                    id,
                    publicPem,
                    privatePem,
                    KeyType,
                    Algorithm,
                    keyAuth,
                    expirationTimeInDays);
            
            }
////////////////////////////////////////////////////////////////////////////////////////////////
                //add also key from ukc
                 //use ukc as keystore
            byte[] keyNameBytes = Encoding.UTF8.GetBytes("test1");
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
                string uid = "9b456af8aac4fbb7";
                //string vOut = (UInt64)privateKeyUid.pValue;


                //build the public key obj
                var publicKeyFromUkc = new PublicKey(nStrBase64,65537);
                
                //publicKeyFromUkc.KeyId = ((UInt64)privateKeyUid.pValue).ToString();
                publicKeyFromUkc.KeyType = "RSA";
                publicKeyFromUkc.Algorithm = "RS256";

                  CreateTestKey(
                    "test1",
                    uid,
                    nStrBase64,
                    nStrBase64,
                    publicKeyFromUkc.KeyType,
                    publicKeyFromUkc.Algorithm,
                    keyAuth,
                    null);
            }


            
////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        }

        public KeyStoreData GetActiveKey(string keyName)
        {
             _logger.LogInformation("call GetActiveKey from testStore class with keyName:" + keyName);
            Dictionary<string, KeyStoreData> keys;
            string activeKey;
            KeyStoreData foundKey;
            if(!this.keys.TryGetValue(keyName, out keys) || !activeKeys.TryGetValue(keyName, out activeKey) ||
                    !keys.TryGetValue(activeKey, out foundKey))
            {
                throw new ArgumentException("Key " + keyName + " not found");
            }

            return foundKey;
        }

        public KeyStoreData GetKey(string keyName, string keyId)
        {
            Console.WriteLine("GetKey function in testStore");
            _logger.LogInformation("call GetKey function in testStore with keyName=" + keyName + "and keyId" + keyId);

            Dictionary<string, KeyStoreData> keys;
            KeyStoreData foundKey;
            if(!this.keys.TryGetValue(keyName, out keys) ||
                    !keys.TryGetValue(keyId, out foundKey))
            {
                throw new ArgumentException("Key " + keyName + "-" + keyId + " not found");
            }

            return foundKey;
        }

        private void CreateTestKey(
            string keyName,
            string keyId,
            string publicKey,
            string privateKey,
            string keyType,
            string algorithm,
            IAuthorizer keyAuth,
            int? expirationTimeInDays)
        {
            keyAuth.ThrowIfNull(nameof(keyAuth));
             _logger.LogInformation("call CreateTestKey function in testStore with keyName=" + keyName + "and keyId" + keyId);

            keys.Add(keyName, new Dictionary<string, KeyStoreData>());

            keys[keyName][keyId] = new KeyStoreData(
                                                new TestKey(publicKey, privateKey),
                                                keyId,
                                                keyType,
                                                algorithm,
                                                keyAuth,
                                                expirationTimeInDays);
            //Multiple keys with the same name can be in the app settings, the first one for the current name is active, the rest have been rolled
            if(!activeKeys.ContainsKey(keyName))
            {
                activeKeys[keyName] = keyId;
            }
        }
    }
}