using System;
using System.Runtime.InteropServices;


namespace unbound.cryptoki {

public class DYCK_FPE_PARAMS : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  public uint ulMode;
  public char[] pFormat;
  public int ulMaxLen;

  public IntPtr ToBin(out int size)
  {
    int ulFormatLen = pFormat==null ? 0 : pFormat.Length+1;
    CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows?16:24, ulFormatLen);
    buf.Pack(ulMode);
    buf.Pack(pFormat);
    buf.Pack(ulMaxLen);
    size = buf.Size;
    return buf.Ptr;
  }
}

}