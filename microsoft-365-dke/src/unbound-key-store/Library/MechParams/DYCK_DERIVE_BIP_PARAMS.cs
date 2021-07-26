using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {
  
public class DYCK_DERIVE_BIP_PARAMS : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  public bool hardened;
  public uint ulChildNumber;  
  
  public IntPtr ToBin(out int size)
  {
    CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows?5:12, 0);
    buf.Pack(hardened);
    buf.Pack(ulChildNumber);
    size = buf.Size;
    return buf.Ptr;
  }
  
}

}