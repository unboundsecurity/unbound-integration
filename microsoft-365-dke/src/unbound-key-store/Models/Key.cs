// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Unbound.Web.Models
{
    public interface IKey
    {
        PublicKey GetPublicKey();
        byte[] Decrypt(byte[] encryptedData);
    }
}