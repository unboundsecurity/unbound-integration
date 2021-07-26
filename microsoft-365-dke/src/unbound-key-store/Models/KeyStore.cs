// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Unbound.Web.Models
{
    public interface IKeyStore
    {
        KeyStoreData GetActiveKey(string keyName);
        KeyStoreData GetKey(string keyName, string keyId);
    }
}