using System;
using System.Linq;
using System.Text;
using unbound.cryptoki;


namespace CustomerKeyStore
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    public static class Program
    {
        public static void Main(string[] args)
        {
            /////////////////////////////////////////
            //  Library.C_Initialize();
            // CK_SLOT_ID[] slots = Library.C_GetSlotList(true);
            // CK_SLOT_ID slot = slots[0];
            // CK_SESSION_HANDLE session = Library.C_OpenSession(slot);

            // CK_OBJECT_HANDLE hKey = Library.C_GenerateKey(session, new CK_MECHANISM(CK.CKM_AES_KEY_GEN), new CK_ATTRIBUTE[] {
            //     new CK_ATTRIBUTE(CK.CKA_TOKEN,     true),
            //     new CK_ATTRIBUTE(CK.CKA_CLASS,     CK.CKO_SECRET_KEY),
            //     new CK_ATTRIBUTE(CK.CKA_KEY_TYPE,  CK.CKK_AES),
            //     new CK_ATTRIBUTE(CK.CKA_VALUE_LEN, 32),
            // });

            // ///////////////////////////////////////
            //     byte[] plainData = Encoding.UTF8.GetBytes("TEST PLAIN DATA");
            //     byte[] keyName = Encoding.UTF8.GetBytes("key1");

            //     //byte[] keyName = "TEST RSA KEY".getBytes("UTF-8");
            //     CK_OBJECT_HANDLE publicTest;
            //     CK_OBJECT_HANDLE tempKey;
            //     CK_OBJECT_HANDLE pubKey;
            //     CK_OBJECT_HANDLE prvKey;
                
            //     // Generate key pair
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
            //         new CK_ATTRIBUTE(CK.CKA_ID, keyName),
            //     },out pubKey,out prvKey);
            //     //int pubKey = keyHandles[0];
            //     //int prvKey = keyHandles[1];


            //     // Find the RSA private key
            //     //System.out.println("Find the RSA private key");
            //     Library.C_FindObjectsInit(session, new CK_ATTRIBUTE[]
            //     {
            //         new CK_ATTRIBUTE(CK.CKA_TOKEN, true),
            //         new CK_ATTRIBUTE(CK.CKA_CLASS, CK.CKO_PRIVATE_KEY),
            //         new CK_ATTRIBUTE(CK.CKA_KEY_TYPE, CK.CKK_RSA),
            //         new CK_ATTRIBUTE(CK.CKA_ID, keyName),
            //     });
            //     CK_OBJECT_HANDLE[] foundKeyHandles = Library.C_FindObjects(session, 1);
            //     Library.C_FindObjectsFinal(session);

            //      CK_ATTRIBUTE n =  new CK_ATTRIBUTE(CK.CKA_MODULUS);
            //      CK_ATTRIBUTE e = new CK_ATTRIBUTE(CK.CKA_PUBLIC_EXPONENT);



            //     //get public key
            //     Library.C_GetAttributeValue(session, foundKeyHandles[0],new CK_ATTRIBUTE[]
            //     {
            //         n,
            //         e,
            //     });

            //      //byte[] eArr =  (byte[]))e.pValue
            //      string nStrBase64 = Convert.ToBase64String((byte[])n.pValue);

            

            //     publicTest = Library.C_CreateObject(session,new CK_ATTRIBUTE[]
            //     {
            //         new CK_ATTRIBUTE(CK.CKA_TOKEN, false),
            //         new CK_ATTRIBUTE(CK.CKA_CLASS, CK.CKO_PUBLIC_KEY),
            //         new CK_ATTRIBUTE(CK.CKA_KEY_TYPE, CK.CKK_RSA),
            //         n,
            //         e
            //     });





            //     //assert foundKeyHandles.length==1;
            //     //System.out.println("Key found - success");                

            // // CK_ATTRIBUTE[] t2 = new CK_ATTRIBUTE[]
            // // {
            // //     new CK_ATTRIBUTE(CK.CKA_ID),
            // //     new CK_ATTRIBUTE(CK.CKA_CLASS),
            // //     new CK_ATTRIBUTE(CK.CKA_KEY_TYPE),
            // // };
            

            // //  Library.C_GetAttributeValue(session, pubKey, t2);

            //     //byte[] vOutPublicKey = BitConverter.GetBytes(pubKey.Handle);
            //     // Convert the array to a base 64 string.
            //     //string s = Convert.ToBase64String(vOutPublicKey);




            //     // Set RSA padding params
            //     //System.out.println("Set RSA padding params");
            //     CK_RSA_PKCS_OAEP_PARAMS oaepParams = new CK_RSA_PKCS_OAEP_PARAMS();
            //     oaepParams.hashAlg = CK.CKM_SHA256;
            //     oaepParams.mgf = CK.CKG_MGF1_SHA256;
            //     CK_MECHANISM mech_rsa = new CK_MECHANISM(CK.CKM_RSA_PKCS_OAEP, oaepParams);
            //       // Encrypt Data
            //     //System.out.println("Encrypt Data");
            //     Library.C_EncryptInit(session, mech_rsa, publicTest);
            //     byte[] encrypted = Library.C_Encrypt(session, plainData);

            //     ulong vIn = 12132554581603151304;
            // uint vOut = (uint)Convert.ToUInt64(vIn);
            // CK_OBJECT_HANDLE hKey2 = new CK_OBJECT_HANDLE(vOut);

            //     // Decrypt Data
            //     //System.out.println("Decrypt Data");
            //     Library.C_DecryptInit(session, mech_rsa, foundKeyHandles[0]);
            //     byte[] decrypted = Library.C_Decrypt(session, encrypted);

            //     //assert Arrays.equals(plainData, decrypted);
            //      if (!Enumerable.SequenceEqual(plainData, decrypted)) throw new Exception("ENC/DEC mismatch");
            //     //System.out.println("Test plain and decrypted data is identical - success");


            // ////////////////////////////////////////

            // //ulong vIn = 12132554581603151304;
            // //uint vOut = (uint)Convert.ToUInt64(vIn);
            // // CK_OBJECT_HANDLE hKey = new CK_OBJECT_HANDLE(vOut);

            // CK_ATTRIBUTE[] t = new CK_ATTRIBUTE[]
            // {
            //     new CK_ATTRIBUTE(CK.CKA_ID),
            //     new CK_ATTRIBUTE(CK.CKA_CLASS),
            //     new CK_ATTRIBUTE(CK.CKA_KEY_TYPE),
            // };
            

            //  Library.C_GetAttributeValue(session, hKey, t);

            // byte[] iv = Library.C_GenerateRandom(session, 12);
            // byte[] plain = Encoding.UTF8.GetBytes("TEST PLAIN DATA");

            // CK_MECHANISM mech = new CK_MECHANISM(CK.CKM_AES_GCM, new CK_GCM_PARAMS(iv, null, 96));
            // Library.C_EncryptInit(session, mech, hKey);
            // byte[] enc = Library.C_Encrypt(session, plain);

            // Library.C_DecryptInit(session, mech, hKey);
            // byte[] dec = Library.C_Decrypt(session, enc);

            // if (!Enumerable.SequenceEqual(dec, plain)) throw new Exception("ENC/DEC mismatch");

            //////////////////////////////////////////
            CreateWebHostBuilder(args).Build().Run();

            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
    }
}
