using Microsoft.Azure.KeyVault.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BlobGettingStarted
{
    class EKMIKeyResolver : IKeyResolver

    {     

        public async Task<IKey> ResolveKeyAsync(string kid, System.Threading.CancellationToken token)
        {
            EKMIkey key = new EKMIkey(kid);
            return await Task.FromResult(key);            
        }
    }
}
