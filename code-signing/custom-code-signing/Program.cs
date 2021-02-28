using System;
using System.Security.Cryptography;

namespace SignApp
{
    class Program
    {
        static void Main(string[] args)
        {          
            CodeSigner.Sign(args[0], args[1], args[2]);
            CNGSigner.Sign();
        }
    }
}
