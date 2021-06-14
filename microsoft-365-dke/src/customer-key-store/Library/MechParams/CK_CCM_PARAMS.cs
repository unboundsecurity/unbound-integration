using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {

  public struct CK_CCM_PARAMS : CK_MECHANISM_PARAM
  {
    private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public int ulDataLen;
    public byte[] pNonce;
    public byte[] pAAD;
    public int ulMACLen;

    public IntPtr ToBin(out int size)
    {
      int ulNonceLen = pNonce == null ? 0 : pNonce.Length;
      int ulAADLen   = pAAD   == null ? 0 : pAAD.Length;
      CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows ? 32:48, ulNonceLen + ulAADLen);
      buf.Pack(ulDataLen);
      buf.Pack(pNonce);
      buf.Pack(ulNonceLen);
      buf.Pack(pAAD);
      buf.Pack(ulAADLen);
      buf.Pack(ulMACLen);
      size = buf.Size;
      return buf.Ptr;
    }
  }


} // namespace unbound.cryptoki