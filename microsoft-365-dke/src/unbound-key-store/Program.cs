using System;
using System.Linq;
using System.Text;
using unbound.cryptoki;
using Newtonsoft.Json.Linq;



namespace UnboundKeyStore
{
     using System.Net;
    using System.IO;
    using System.Net.Http;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
   
 
    public static class Program
    {
        static readonly HttpClient client = new HttpClient();

        public static void Main(string[] args)
        {
            //string websiteHostName = System.Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            // string websiteHostName = "54.174.121.27";

            // // WebClient webClient = new WebClient();
            // // webClient.QueryString.Add("param1", "value1");
            // // webClient.QueryString.Add("param2", "value2");
            // //string result = webClient.DownloadString("https://" + websiteHostName);
            // //Console.WriteLine(result);

            // ///////////END GET ACCESS TOKEN FROM UKC///////////////////////////////

            // var postData = "grant_type=" + Uri.EscapeDataString("password");
            // postData += "&username=" + Uri.EscapeDataString("so@part1");
            // postData += "&password=" + Uri.EscapeDataString("Password1!");

            // var data = Encoding.ASCII.GetBytes(postData);

            // HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://" + websiteHostName + "/api/v1/token" );
            // request.Method = "POST";
            // request.KeepAlive = true;
            // //request.ContentType = "appication/json";
            // request.ContentType = "application/x-www-form-urlencoded";
            // request.ContentLength = data.Length;
            // //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            // //request.ContentType = "application/x-www-form-urlencoded";
            // request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            // using (var stream = request.GetRequestStream())
            // {
            //     stream.Write(data, 0, data.Length);
            // }

            // HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            // string myResponse = "";
            // using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            // {
            //     myResponse = sr.ReadToEnd();
            // }
            // Console.Write(myResponse);
            // JObject json = JObject.Parse(myResponse);
            // string accessToken = "";
            // foreach (var e in json)
            // {
            //     if(e.Key=="access_token")
            //     {
            //         accessToken = e.Value.ToString();
            //     }
            // }

            // ///////////END GET ACCESS TOKEN FROM UKC///////////////////////////////



            // ////////////GET KEY FROM UKC USING REST//////////////////////////////////
            // //postData = "partitionId=" + Uri.EscapeDataString("part1");
            // //postData += "&username=" + Uri.EscapeDataString("so@test");
            // //postData += "&password=" + Uri.EscapeDataString("Unbound1!");

            // //data = Encoding.ASCII.GetBytes(postData);
            // var keyUid = "0x00b0071ba7dc79f9bf";
            // var partitionId = "part1";

            // HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create("https://" + websiteHostName + "/api/v1/keys/" + keyUid + "?partitionId=" + partitionId);
            // request2.Method = "GET";
            // request2.KeepAlive = true;
            // request.ContentType = "appication/json";
            // //request.ContentType = "application/x-www-form-urlencoded";
            // //request.ContentLength = data.Length;
            // //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            // //request.ContentType = "application/x-www-form-urlencoded";
            // request2.PreAuthenticate = true;
            // request2.Headers.Add("Authorization", "Bearer " + accessToken);
            // request2.Accept = "application/json";

            // request2.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            // // using (var stream = request2.GetRequestStream())
            // // {
            // //     stream.Write(data, 0, data.Length);
            // // }

            // HttpWebResponse response2 = (HttpWebResponse)request2.GetResponse();
            
            // string myResponse2 = "";
            // using (System.IO.StreamReader sr = new System.IO.StreamReader(response2.GetResponseStream()))
            // {
            //     myResponse2 = sr.ReadToEnd();
            // }
            // Console.Write(myResponse2);

            // ////////////END GET KEY FROM UKC USING REST//////////////////////////////////

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
