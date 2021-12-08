// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Unbound.Web.Controllers
{
    using System;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System.Web;
    using System.Collections.Specialized;
    using System.Text;
    using System.Security.Claims;
    using System.Net.Http;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using System.Net;
    using System.IO;
    using ippw = Unbound.Web.Models;
    using Newtonsoft.Json.Linq;
    
    public class KeysController : Controller
    {
        private readonly ippw.KeyManager keyManager;

         private readonly ILogger _logger;


        public KeysController(ippw.KeyManager keyManager,ILogger<KeysController> logger)
        {
            this.keyManager = keyManager;
            _logger = logger;

        }

         public void PrintRequestHeaders()
         {
            var builder = new StringBuilder(Environment.NewLine);
            foreach (var header in Request.Headers)
            {
                builder.AppendLine($"{header.Key}: {header.Value}");
            }
            var headersDump = builder.ToString();
            _logger.LogInformation(headersDump);
         }

         public void PrintUserClaims()
         {
            ClaimsPrincipal user = HttpContext.User;
            foreach(var claim in user.Claims)
            {
                _logger.LogInformation("found  user claim : " + claim.Value);
            }
         }

        [HttpGet]
        public IActionResult GetKey(string keyName)
        {
            try
            {
                PrintRequestHeaders(); 
                PrintUserClaims();
                var publicKey = keyManager.GetPublicKey(Request,keyName);

                return Ok(publicKey);
            }
            catch(WebException ex)
            {
                var errorMessage = ex.Message;
                _logger.LogInformation("Error message : " + errorMessage);
                if(ex.Response != null) {
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        JObject json = JObject.Parse(reader.ReadToEnd());
                        errorMessage = (string)json.SelectToken("message");
                    }
                }
                 _logger.LogInformation("Error message : " + errorMessage);

                var wRespStatusCode = ((HttpWebResponse)ex.Response).StatusCode;
                if(wRespStatusCode==HttpStatusCode.Unauthorized)
                {
                    _logger.LogInformation("Error in authroization header – sending unauthorized request to UKC. Use env UKC_USER / UKC_PASSWORD");
                    return StatusCode(401, "Authroization failed – please check the request header credentials");
                }

                return StatusCode(400,"Invalid configuration or missing parameter. Check with system administrator");
            }
            catch(UnboundKeyStore.Models.KeyAccessException)
            {
                return StatusCode(403);
            }
            catch(ArgumentException e)
            {
                 _logger.LogInformation(e.ToString());
                return StatusCode(400,e.Message);
            }
        }

        [HttpPost]
        public IActionResult Decrypt(string keyName, string keyId, [FromBody] ippw.EncryptedData encryptedData)
        {
            try
            {
                PrintRequestHeaders(); 
                PrintUserClaims(); 
                var decryptedData = keyManager.Decrypt(Request,keyName, keyId, encryptedData);
                return Ok(decryptedData);
            }
            
            catch(WebException ex)
            {
                var errorMessage="";
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    JObject json = JObject.Parse(reader.ReadToEnd());
                    errorMessage = (string)json.SelectToken("message");
                }
                 _logger.LogInformation("Error message : " + errorMessage);

                var wRespStatusCode = ((HttpWebResponse)ex.Response).StatusCode;
                if(wRespStatusCode==HttpStatusCode.Unauthorized)
                {
                     _logger.LogInformation("Error in authroization header – sending unauthorized request to UKC. Use env UKC_USER / UKC_PASSWORD");
                    return StatusCode(401, "Authroization failed – please check the request header credentials");
                }

                return StatusCode(400,"Invalid configuration or missing parameter. Check with system administrator");
            }
            
            catch(UnboundKeyStore.Models.KeyAccessException)
            {
                return StatusCode(403);
            }
            catch(ArgumentException e)
            {
                 _logger.LogInformation(e.ToString());
                return StatusCode(400,e.Message);
            }
        }

        private static Uri GetRequestUri(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            return new Uri(request.GetDisplayUrl());
        }
    }
}
