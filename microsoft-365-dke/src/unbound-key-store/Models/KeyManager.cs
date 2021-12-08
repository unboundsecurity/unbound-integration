// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Unbound.Web.Models
{
    using System;
    using System.Web;

    using System.Security.Claims;
    using Unbound.Web.Models.Extensions;
    using sg = System.Globalization;

    using System.Linq;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using System.Net.Http;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using System.Net;
    using System.IO;
    using Newtonsoft.Json.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json;


    public class KeyManager
    {
        private readonly ILogger _logger;
        string UKC_URL = System.Environment.GetEnvironmentVariable("UKC_URL") + "/api/v1";
        string partition = System.Environment.GetEnvironmentVariable("UKC_PARTITION");

        string BASIC_AUTH;
        public KeyManager(ILogger<KeyManager> logger)
        {
            _logger = logger;

            string userName = System.Environment.GetEnvironmentVariable("UKC_USER_NAME");
            string passowrd = System.Environment.GetEnvironmentVariable("UKC_PASSWORD");
            string encoded = System.Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1")
                               .GetBytes(userName + "@" + partition + ":" + passowrd));

            BASIC_AUTH = "Basic" + encoded;
        }

        // Try to extract the Bearer token from the request
        // this is used for proxying the token to UKC to use it for authorization
        // in order for this to work, UKC OpenID should be configured
        public string getTokenFromHeaders(Microsoft.AspNetCore.Http.HttpRequest request, Boolean shouldSearchMicrosoftToken)
        {
            var accessToken = "";
            foreach (var header in request.Headers)
            {
                if (header.Key == "X-MS-TOKEN-AAD-ID-TOKEN" && shouldSearchMicrosoftToken)
                {
                    accessToken = "Bearer " + header.Value;
                    _logger.LogInformation("Found token from microsoft : " + accessToken);

                }
                else if (header.Key == "Authorization")
                {
                    accessToken = header.Value;
                }
            }

            return accessToken;
        }

        public string getToken(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            string accessToken = "";
 

            accessToken = getTokenFromHeaders(request, false);
            if (String.IsNullOrEmpty(accessToken))
            {
                accessToken = BASIC_AUTH;
            }

            return accessToken;

        }

        public String getUkcToken()
        {
            string password = System.Environment.GetEnvironmentVariable("UKC_PASSWORD");
            string userName = System.Environment.GetEnvironmentVariable("UKC_USER_NAME");

            var postData = "grant_type=" + Uri.EscapeDataString("password");
            postData += "&username=" + Uri.EscapeDataString(userName + "@" + partition);
            postData += "&password=" + Uri.EscapeDataString(password);

            var data = Encoding.ASCII.GetBytes(postData);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UKC_URL + "/token");
            request.Method = "POST";
            request.KeepAlive = true;
            //request.ContentType = "appication/json";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            //request.ContentType = "application/x-www-form-urlencoded";

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
                if (e.Key == "access_token")
                {
                    accessToken = e.Value.ToString();
                }
            }

            return accessToken;

        }

        public byte[] HexStringToBin(string inputHex)
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
            return System.Convert.ToBase64String(HexStringToBin(input));
        }

        public JObject SendUkcRequest(Microsoft.AspNetCore.Http.HttpRequest requestContext, string method, string path, object body = null) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UKC_URL + "/" + path + "?partitionId=" + partition);
            request.Method = method;
            request.KeepAlive = true;
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", getToken(requestContext));
            request.Accept = "application/json";
            request.ContentType = "application/json; charset=utf-8";

            if( body != null ) {
                var jsonbody = JsonConvert.SerializeObject(body);
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonbody);
                    streamWriter.Flush();
                }
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string responseData = "";
            using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            {
                responseData = sr.ReadToEnd();
            }
            JObject json = JObject.Parse(responseData);
            return json;
        }

        public KeyData GetPublicKey(Microsoft.AspNetCore.Http.HttpRequest request, string keyName)
        {
            _logger.LogInformation("get public key : " + keyName);
            keyName.ThrowIfNull(nameof(keyName));

            JObject json = SendUkcRequest(request, "GET", "keys/" + keyName);
            if(json.SelectToken("pkInfo.rsa.publicExponent")==null) {
                throw new System.ArgumentException("key " + keyName + " not found");
            }
            string publicExponent = (string)json.SelectToken("pkInfo.rsa.publicExponent");
            string keyUid = (string)json.SelectToken("uid"); 
            string modulus = (string)json.SelectToken("pkInfo.rsa.modulus");
            string hexstr = modulus.Replace(":", "", System.StringComparison.InvariantCulture);
            string hexstrWithoutPrefixZero = hexstr.Substring(2, hexstr.Length - 2);
            string nStrBase64 = HexString2B64String(hexstrWithoutPrefixZero);
            var publicKey = new PublicKey(nStrBase64, 65537);
            string appHostName = System.Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            publicKey.KeyId = "https://" + appHostName + "/" + keyName + "/" + keyUid;
            publicKey.KeyType = "RSA";
            publicKey.Algorithm = "RS256";

            return new KeyData(publicKey);
        }

        public DecryptedData Decrypt(Microsoft.AspNetCore.Http.HttpRequest httpRequest, string keyName, string keyId, EncryptedData encryptedData)
        {

            _logger.LogInformation("decrypt called from key manager class for keyName : " + keyName + " and keyID : " + keyId);
            string myResponse = string.Empty;
            string clearText = string.Empty;


            keyName.ThrowIfNull(nameof(keyName));
            keyId.ThrowIfNull(nameof(keyId));
            encryptedData.ThrowIfNull(nameof(encryptedData));

            string cipherTextBase64 = encryptedData.Value;

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

            try
            {
                JObject jsonObj = SendUkcRequest(httpRequest, "POST", "keys/" + keyId + "/decrypt", body);
                clearText = (string)jsonObj.SelectToken("clearText");
            }
            catch (WebException e)
            {
                _logger.LogInformation("This program is expected to throw WebException on successful run." +
                                   "\n\nException Message :" + e.Message);
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    _logger.LogInformation("Status Code : {0}", ((HttpWebResponse)e.Response).StatusCode);
                    _logger.LogInformation("Status Description : {0}", ((HttpWebResponse)e.Response).StatusDescription);
                    throw e;
                }
            }

            return new DecryptedData(clearText);
        }
    }
}