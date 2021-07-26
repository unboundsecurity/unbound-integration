using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {

public class CK_KEY_DERIVATION_STRING_DATA : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  public byte[] pData;

  public IntPtr ToBin(out int size)
  {
    int ulDataLen  = pData  == null ? 0 : pData.Length;
    CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows?12:16, ulDataLen);
    buf.Pack(pData);
    buf.Pack(ulDataLen);
    size = buf.Size;
    return buf.Ptr;
  }
}

}