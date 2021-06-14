using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {

public class CK_RSA_PKCS_PSS_PARAMS : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  public uint hashAlg;
  public uint mgf;
  public int sLen;

  public IntPtr ToBin(out int size)
  {
    CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows?12:24, 0);
    buf.Pack(hashAlg);
    buf.Pack(mgf);
    buf.Pack(sLen);
    size = buf.Size;
    return buf.Ptr;
  }
}

}