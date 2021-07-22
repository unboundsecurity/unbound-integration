using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {

public class DYCK_SPE_PARAMS : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  public int ulBits;

  public IntPtr ToBin(out int size)
  {
    CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows?4:8, 0);
    buf.Pack(ulBits);
    size = buf.Size;
    return buf.Ptr;
  }
}

}