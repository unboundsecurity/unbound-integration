using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {

public class CK_ECDH1_DERIVE_PARAMS : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  public uint kdf;
  public byte[] pSharedData;
  public byte[] pPublicData;

  public IntPtr ToBin(out int size)
  {
    int ulSharedDataLen = pSharedData == null ? 0 : pSharedData.Length;
    int ulPublicDataLen = pPublicData == null ? 0 : pPublicData.Length;
    CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows ? 28 : 40, ulSharedDataLen + ulPublicDataLen);
    buf.Pack(kdf);
    buf.Pack(ulSharedDataLen);
    buf.Pack(pSharedData);
    buf.Pack(ulPublicDataLen);
    buf.Pack(pPublicData);
    size = buf.Size;
    return buf.Ptr;
  }
}

} // namespace unbound.cryptoki