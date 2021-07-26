using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki 
{
public class CK_RSA_PKCS_OAEP_PARAMS : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  public uint hashAlg;
  public uint mgf;
  public uint source;
  public byte[] pSourceData;

  public IntPtr ToBin(out int size)
  {
    int ulSourceDataLen  = pSourceData  == null ? 0 : pSourceData.Length;
    CK_MECHANISM_PARAM_Buffer buf = new CK_MECHANISM_PARAM_Buffer(isWindows?24:40, ulSourceDataLen);
    buf.Pack(hashAlg);
    buf.Pack(mgf);
    buf.Pack(source);
    buf.Pack(pSourceData);
    buf.Pack(ulSourceDataLen);
    size = buf.Size;
    return buf.Ptr;
  }
}

}