using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {

public class CK_GCM_PARAMS : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

  public byte[] pIv;
  public byte[] pAAD;
  public int ulTagBits;

  public CK_GCM_PARAMS(byte[] pIv, byte[] pAAD, int ulTagBits)
  {
    this.pIv = pIv;
    this.pAAD = pAAD;
    this.ulTagBits = ulTagBits;
  }
  
  public IntPtr ToBin(out int size)
  {
    int ulIvLen  = pIv  == null ? 0 : pIv.Length;
    int ulAADLen = pAAD == null ? 0 : pAAD.Length;
    CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows ? 28:40, ulIvLen + ulAADLen);
    buf.Pack(pIv);
    buf.Pack(ulIvLen);
    buf.Pack(pAAD);
    buf.Pack(ulAADLen);
    buf.Pack(ulTagBits);
    size = buf.Size;
    return buf.Ptr;
  }

}

}