
using System;
using System.Security.Cryptography;


namespace SignApp
{
    class CNGSigner
    {
        public static void Sign()
        {
            CngProvider provider = new CngProvider("Dyadic Security Key Storage Provider");

            // get the key       
            RSACng privateKey = new RSACng(CngKey.Open("test", provider));

            // sign with the key
            byte[] message = new byte[32];
            byte[] signature = privateKey.SignData(message, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

            Console.WriteLine("Message :" + Convert.ToBase64String(message));
            Console.WriteLine("Signature :" + Convert.ToBase64String(signature));
        }
    }
}
