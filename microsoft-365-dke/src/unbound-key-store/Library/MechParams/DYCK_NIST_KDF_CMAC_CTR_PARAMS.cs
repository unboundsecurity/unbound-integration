using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {

public class DYCK_NIST_KDF_CMAC_CTR_PARAMS : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  public byte[] pLabel;
  public byte[] pContext;
  public int ulSecretLen;
  public IntPtr ToBin(out int size)
  {
    int ulLabelLen   = pLabel   == null ? 0 : pLabel.Length;
    int ulContextLen = pContext == null ? 0 : pContext.Length;
    CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows?28:40, ulLabelLen + ulContextLen);
    buf.Pack(pLabel);
    buf.Pack(ulLabelLen);
    buf.Pack(pContext);
    buf.Pack(ulContextLen);
    buf.Pack(ulSecretLen);
    size = buf.Size;
    return buf.Ptr;
  }
}

}