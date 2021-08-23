// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Unbound.Web.Models
{
    using System;
    using System.Security.Claims;
    using Unbound.Web.Models.Extensions;
    using sg = System.Globalization;
    using unbound.cryptoki;

    using System.Linq;
    using System.Text;
    using Microsoft.Extensions.Logging;
         using System.Net.Http;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
         using System.Net;
    using System.IO;
     using Newtonsoft.Json.Linq;
     using System.Text.Json;

     using System.Text.Json.Serialization;
     using Newtonsoft.Json;



    public class KeyManager
    {
        private readonly ILogger _logger;

        public KeyManager(ILogger<KeyManager> logger)
        {
             _logger = logger;
        }

        public String getUkcToken()
        {
            string websiteHostName = System.Environment.GetEnvironmentVariable("UKC_SERVER_IP");
            string partition =  System.Environment.GetEnvironmentVariable("UKC_PARTITION");
            string password =  System.Environment.GetEnvironmentVariable("UKC_PASSWORD");

            
            var postData = "grant_type=" + Uri.EscapeDataString("password");
            postData += "&username=" + Uri.EscapeDataString("so@" + partition);
            postData += "&password=" + Uri.EscapeDataString(password);

            var data = Encoding.ASCII.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + websiteHostName + "/api/v1/token" );
            request.Method = "POST";
            request.KeepAlive = true;
            //request.ContentType = "appication/json";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //request.ContentType = "application/x-www-form-urlencoded";
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            string myResponse = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                myResponse = sr.ReadToEnd();
            }
            Console.Write(myResponse);
            JObject json = JObject.Parse(myResponse);
            string accessToken = "";
            foreach (var e in json)
            {
                if(e.Key=="access_token")
                {
                    accessToken = e.Value.ToString();
                }
            }

                return accessToken;        

        }

        public string ASCIITOHex(string ascii)

        {

            StringBuilder sb = new StringBuilder();

            byte[] inputBytes = Encoding.UTF8.GetBytes(ascii);

            foreach (byte b in inputBytes)

            {

                sb.Append(string.Format("{0:x2}", b));

            }

            return sb.ToString();

        }

        public  byte[] HexStringToHex(string inputHex)
        {
            var resultantArray = new byte[inputHex.Length / 2];
            for (var i = 0; i < resultantArray.Length; i++)
            {
                resultantArray[i] = System.Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
            }
            return resultantArray;
        }


        public string HexString2B64String(string input)
        {
            return System.Convert.ToBase64String(HexStringToHex(input));
        }

        public KeyData GetPublicKey(string accessToken, string keyName)
        {
            _logger.LogInformation("get public key : " + keyName );

            //requestUri.ThrowIfNull(nameof(requestUri));
            keyName.ThrowIfNull(nameof(keyName));
            // if(System.Environment.GetEnvironmentVariable("ENV")=="dev")
            // {
            //     _logger.LogInformation("USING UKC TOKEN FOR DEV");
            //     accessToken = getUkcToken();
            // }
            
            ////////////GET KEY FROM UKC USING REST//////////////////////////////////
            string websiteHostName = System.Environment.GetEnvironmentVariable("UKC_SERVER_IP");
            //TODO: get this uid from ListKeys API
            var keyUid = "0x00b0071ba7dc79f9bf";
            string partition =  System.Environment.GetEnvironmentVariable("UKC_PARTITION");

            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create("https://" + websiteHostName + "/api/v1/keys/" + keyUid + "?partitionId=" + partition);
            request2.Method = "GET";
            request2.KeepAlive = true;
            //request.ContentType = "appication/json";
            //request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = data.Length;
            //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //request.ContentType = "application/x-www-form-urlencoded";
            request2.PreAuthenticate = true;
            request2.Headers.Add("Authorization", accessToken);
            request2.Accept = "application/json";

            request2.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();
            
            string myResponse2 = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response2.GetResponseStream()))
            {
                myResponse2 = sr.ReadToEnd();
            }
             _logger.LogInformation("UKC response to getKey : " + myResponse2);

            JObject json = JObject.Parse(myResponse2);
            string publicExponent = (string)json.SelectToken("pkInfo.rsa.publicExponent");
            string modulus = (string)json.SelectToken("pkInfo.rsa.modulus");
            string nStrBase64 = HexString2B64String(modulus.Replace(":",""));

            //string nStrBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(modulus));

           // string nStrBase64 = "quZmxXahDC8x1tlbWw2+UV5jbZeCmQMzru3VftfIaxLvJQ8ft0fuaQo5LlPxtzxXNuPe3Ew55Qgxx0dLf24zwWA3cfnfUhxuNnTWaSGgrT18TKH8hD0wJ/d6QNSDmtSdnZyinp+XHixe0ifn+MydgtKjYp0UKBIMVS7e8GIwJPK48SFqWxmmGIxTW5+sMLosAJ28Co/00+vqPolXHX7uDnNfkoYfWfAgkdWIfK4ScKfu/1JmMxQTr4MFBLv1SmusR3ypyIp/mLL+MYPevYmqVFmCHK+yjNvAIPpissZ+tlzcpx8hqzdb2uDrTd9DkWa0OWGFGKv1qsoySfiZtuEUHw==";
            var publicKey = new PublicKey(nStrBase64,65537);
            websiteHostName = System.Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            publicKey.KeyId = "https://" + websiteHostName + "/" + keyName  + "/" + keyUid;
            publicKey.KeyType = "RSA";
            publicKey.Algorithm = "RS256";

            return new KeyData(publicKey);
              
        }

        public DecryptedData Decrypt(string accessToken, string keyName, string keyId, EncryptedData encryptedData)
        {
            _logger.LogInformation("decrypt called from key manager class for keyName : " + keyName + " and keyID : " + keyId );
            //user.ThrowIfNull(nameof(user));
            string myResponse = "";
            string clearText="";

            // if(System.Environment.GetEnvironmentVariable("ENV")=="dev")
            // {
            //     _logger.LogInformation("USING UKC TOKEN FOR DEV");
            //     accessToken = "eyJraWQiOiIweDAwNWY4ZmZjZDczYTA4MTNlYSIsImFsZyI6IkVTMjU2In0.eyJwYXJ0aXRpb25zIjp7InBhcnQxIjpbImRrZSJdfSwic3ViIjoibWljaGFlbEB1bmJvdW5kbGFiLm9ubWljcm9zb2Z0LmNvbUBwYXJ0MSIsIm9yaWciOiI4Ny43MS4xNjYuNzEiLCJpc3MiOiJVTkJPVU5EIiwiaXNfcmVmcmVzaCI6ZmFsc2UsInVzZV9lcGhlbWVyYWwiOnRydWUsImV4cCI6MTYyOTYzNTc2OSwiaWF0IjoxNjI5NjMzOTY5LCJqdGkiOiJjNGVhODUyOS04Mjg0LTQ5YjUtODExOS03MDNkMjE1NGFhODYifQ.OlQEhaBvxV8oPGbEJ33JQGGHMbeqsAg_hRNONTmOTU0S4I6TOEFkZ3slmDwjJTXHuZWqWzZuDK743k-sl4M-iA";
            // }

            keyName.ThrowIfNull(nameof(keyName));
            keyId.ThrowIfNull(nameof(keyId));
            encryptedData.ThrowIfNull(nameof(encryptedData));

            string websiteHostName = System.Environment.GetEnvironmentVariable("UKC_SERVER_IP");
            string partition =  System.Environment.GetEnvironmentVariable("UKC_PARTITION");
            //string cipherTextBase64 = "CSYMkVvmsD9e/bLxPoT2c7CSPwM/Y+qJ5PU4vATLRccSdKhECzPg1gNHw67os6o2/H3Y41VN4nR+Augo2aNytqvvc4STqch6PZhF8z8SZgf/MW04KfgVFQQYIEDFQ+QI0B2SrY44wqnqpBN0cClcl1JAJoXAwWDVLm72AML45e3yRJbv7rgWgmLw9uYXrW66E2u9X2nyb5doW+Cxc3/Rrv8b6nGVnhtwnAPj2o0QdDsc7lnLPT2UAG8vImfa5bsG6xHYIe8J723VG6Wm7RqzdHPb9LG1e9EiWG331K+4lPpGInPzgnwsUse0T4NMswo+qcFTVEbC0LFUpgNMmGqhOw==";
            string cipherTextBase64 = encryptedData.Value;

            //create decypt body

            var body = new
            {
                cipher = new
                {
                    cipherTextBase64 = cipherTextBase64                   
                },
                aSymmetricParams = new
                {
                    padding = new
                    {
                        type = "OAEP",
                        oaep = new 
                        {
                            mgf = "SHA256"
                        }
                    },
                },
                outputEncoding = "BASE64"
            };
           // JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
           // string json = JsonSerializer.Serialize(body);
            var json = JsonConvert.SerializeObject(body);
            var data = Encoding.ASCII.GetBytes(json);
            //var keyUid = "b0071ba7dc79f9bf";

            try{
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + websiteHostName + "/api/v1/keys/" + keyId + "/decrypt" + "?partitionId=" + partition);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.KeepAlive = true;
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", accessToken);
            request.Accept = "application/json";
            //request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = data.Length;
            request.Headers.Add("Accept", "application/json");
            //request.ContentType = "application/x-www-form-urlencoded";
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                //string json = "{ \"method\" : \"guru.test\", \"params\" : [ \"Guru\" ], \"id\" : 123 }";

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                myResponse = sr.ReadToEnd();
            }
            Console.Write(myResponse);

            JObject jsonObj = JObject.Parse(myResponse);
            clearText = (string)jsonObj.SelectToken("clearText");
            //string modulus = (string)jsonObj.SelectToken("pkInfo.rsa.modulus");
            }
            catch(WebException e) {
                 _logger.LogInformation("This program is expected to throw WebException on successful run."+
                                    "\n\nException Message :" + e.Message);
                if(e.Status == WebExceptionStatus.ProtocolError) {
                     _logger.LogInformation("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                     _logger.LogInformation("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    throw e;
                }
            }
            // JObject json = JObject.Parse(myResponse);
            // string accessToken = "";
            // foreach (var e in json)
            // {
            //     if(e.Key=="access_token")
            //     {
            //         accessToken = e.Value.ToString();
            //     }
            // }


            //use ukc to decrypt
            // byte[] keyNameBytes = Encoding.UTF8.GetBytes(keyName);
            // ulong keyUID = (ulong)Convert.ToUInt64(keyId,16);

            // CK_OBJECT_HANDLE pubKey;
            // CK_OBJECT_HANDLE prvKey;
            // CK_OBJECT_HANDLE publicTest;

            // Library.C_Initialize();
            // CK_SLOT_ID[] slots = Library.C_GetSlotList(true);
            // CK_SLOT_ID slot = slots[0];
            // CK_SESSION_HANDLE session = Library.C_OpenSession(slot);

            // Library.C_FindObjectsInit(session, new CK_ATTRIBUTE[]
            // {
            //     new CK_ATTRIBUTE(CK.CKA_TOKEN, true),
            //     new CK_ATTRIBUTE(CK.CKA_CLASS, CK.CKO_PRIVATE_KEY),
            //     new CK_ATTRIBUTE(CK.CKA_KEY_TYPE, CK.CKK_RSA),
            //     //new CK_ATTRIBUTE(CK.CKA_ID, keyNameBytes),
            //     new CK_ATTRIBUTE(CK.DYCKA_UID , keyUID)

            // });

            // CK_OBJECT_HANDLE[] foundKeyHandles = Library.C_FindObjects(session, 1);
            // Library.C_FindObjectsFinal(session);

            // CK_ATTRIBUTE n =  new CK_ATTRIBUTE(CK.CKA_MODULUS);
            // CK_ATTRIBUTE e = new CK_ATTRIBUTE(CK.CKA_PUBLIC_EXPONENT);
            // CK_ATTRIBUTE privateKeyUid =  new CK_ATTRIBUTE(CK.DYCKA_UID); 

            // if(foundKeyHandles.Length == 0) throw new Exception("key" + keyName + " not found");

            // //_logger.LogInformation("encryptedData.Value = "  + encryptedData.Value);

            // byte[] plainData = Convert.FromBase64String(encryptedData.Value);

            // CK_RSA_PKCS_OAEP_PARAMS oaepParams = new CK_RSA_PKCS_OAEP_PARAMS();
            // oaepParams.hashAlg = CK.CKM_SHA256;
            // oaepParams.mgf = CK.CKG_MGF1_SHA256;
            // CK_MECHANISM mech_rsa = new CK_MECHANISM(CK.CKM_RSA_PKCS_OAEP, oaepParams);
            
            // Library.C_DecryptInit(session, mech_rsa, foundKeyHandles[0]);

            // byte[] decrypted = Library.C_Decrypt(session, plainData);



            return new DecryptedData(clearText);        
        }
    }
}