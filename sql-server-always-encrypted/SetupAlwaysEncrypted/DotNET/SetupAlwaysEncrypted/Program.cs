using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace AlwaysEncryptedKeysUsingCsp
{
    class Program
    {
        static void Main(string[] args)
        {
            string providerName = "Dyadic Security Key Storage Provider";
            string keyContainerName = args[0];

            // Create a CMK using CSP
/*            if (!CreateCmkUsingCsp(providerName, keyContainerName))
            {
                throw new Exception(@"Failed to create the column master key inside EKM");
            } */

            // Create and encrypt a CEK
            byte[] encryptedCek = CreateEncryptedCekUsingCsp(providerName, keyContainerName);
            if (encryptedCek == null)
            {
                throw new Exception(@"Failed to create th encrypted column encryption key");
            }
            //Console.WriteLine(@"{0}", ConvertBytesToHexString(encryptedCek, true));
            Console.WriteLine(@"Encrypted CEK: {0}", ConvertBytesToHexString(encryptedCek, true));
            Console.ReadKey();
        }

        internal static byte[] CreateEncryptedCekUsingCsp(string providerName, string keyContainerName)
        {
            try
            {
                // Create a random column encryption key of size 256 bits
                byte[] columnEncryptionKey = GenerateRandomBytes(32);
                Console.WriteLine(@"Plaintext CEK: {0}", ConvertBytesToHexString(columnEncryptionKey, true));

                // Encrypt CEK with CMK stored in EKM
                string keyPath = String.Format(@"{0}/{1}", providerName, keyContainerName);
                SqlColumnEncryptionCngProvider cspProvider = new SqlColumnEncryptionCngProvider();

                return cspProvider.EncryptColumnEncryptionKey(keyPath, @"RSA_OAEP", columnEncryptionKey);
            }
            catch (Exception e)
            {
                Console.WriteLine("\tFAILURE: Creating the encrypted column encryption key failed");
                Console.WriteLine(@"    {0}", e.Message);
                return null;
            }
        }

        /// <summary>
        /// Creates an RSA 2048 key inside the specified CSP.
        /// </summary>
        /// <param name="providerName">CSP name</param>
        /// <param name="containerName">Container name</param>
        /// <returns></returns>
        internal static bool CreateCmkUsingCsp(string providerName, string containerName)
        {
            try
            {
                // open provider
                CngProvider provider = new CngProvider(providerName);

                // generate new test key pair
                CngKeyCreationParameters creation = new CngKeyCreationParameters();
                creation.Provider = provider;
                CngKey privateKey = CngKey.Create(new CngAlgorithm("RSA"), containerName, creation); // default is 2048
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("\tFAILURE: The RSA key was not persisted in the container, \"{0}\".", containerName);
                Console.WriteLine(@"    {0}", e.Message);
                return false;
            }

            return true;
        }

        internal static byte[] GenerateRandomBytes(int length)
        {
            // Generate random bytes cryptographically.
            byte[] randomBytes = new byte[length];
            RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
            rngCsp.GetBytes(randomBytes);

            return randomBytes;
        }

        /// <summary>
        /// Gets hex representation of byte array.
        /// <param name="input">input byte array</param>
        /// <param name="addLeadingZeroX">Add leading 0x</param>
        /// </summary>
        internal static string ConvertBytesToHexString(byte[] input, bool addLeadingZeroX = false)
        {
            StringBuilder str = new StringBuilder();
            if (addLeadingZeroX)
            {
                str.Append(@"0x");
            }

            foreach (byte b in input)
            {
                str.AppendFormat(b.ToString(@"X2"));
            }

            return str.ToString();
        }
    }
}
