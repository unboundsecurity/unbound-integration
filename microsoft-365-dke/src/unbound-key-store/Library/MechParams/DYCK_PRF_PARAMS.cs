using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {

public class DYCK_PRF_PARAMS : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  public uint ulPurpose;
  public byte[] pTweak;
  public int ulSecretLen;

  public IntPtr ToBin(out int size)
  {
    int ulTweakLen   = pTweak   == null ? 0 : pTweak.Length;
    CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows?20:32, ulTweakLen);
    buf.Pack(ulPurpose);
    buf.Pack(pTweak);
    buf.Pack(ulTweakLen);
    buf.Pack(ulSecretLen);
    size = buf.Size;
    return buf.Ptr;
  }
}

}