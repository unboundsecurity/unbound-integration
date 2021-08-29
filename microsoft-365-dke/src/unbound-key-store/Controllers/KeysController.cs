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
       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetKey(string keyName)
        {
            try
            {
                PrintRequestHeaders(); 
                PrintUserClaims();
                var publicKey = keyManager.GetPublicKey(Request,keyName);

                return Ok(publicKey);
            }
            catch(UnboundKeyStore.Models.KeyAccessException)
            {
                return StatusCode(403);
            }
            catch(ArgumentException e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Decrypt(string keyName, string keyId, [FromBody] ippw.EncryptedData encryptedData)
        {
            try
            {
                PrintRequestHeaders(); 
                PrintUserClaims(); 
                var decryptedData = keyManager.Decrypt(Request,keyName, keyId, encryptedData);
                return Ok(decryptedData);
            }
            catch(UnboundKeyStore.Models.KeyAccessException)
            {
                return StatusCode(403);
            }
            catch(ArgumentException e)
            {
                return BadRequest(e);
            }
        }

        private static Uri GetRequestUri(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            return new Uri(request.GetDisplayUrl());
        }
    }
}
