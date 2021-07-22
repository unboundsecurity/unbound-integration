using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {

public class DYCK_AES_SIV_PARAMS : CK_MECHANISM_PARAM
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  public byte[][] pAuthData;

  public IntPtr ToBin(out int size)
  {
    int count = pAuthData==null ? 0 : pAuthData.Length;
    int authDataLen = 0;
    for (int i = 0; i < count; i++) authDataLen += pAuthData[i].Length;
    size = isWindows?12:16;

    IntPtr ptr = Marshal.AllocCoTaskMem(size + count*12 + authDataLen);
    int datasOffset = isWindows?12:16;
    int authOffset = (isWindows?12:16) + (isWindows?12:16)*count;
    IntPtr datas = new IntPtr(ptr.ToInt64() + datasOffset);
    if (isWindows) Marshal.WriteInt32(ptr, 0, count);
    else Marshal.WriteInt64(ptr, 0, count);
    Marshal.WriteIntPtr(ptr, 4, datas);

    for (int i = 0; i < count; i++, datasOffset+=12)
    {
      IntPtr pAuth = new IntPtr(ptr.ToInt64() + authOffset);
      if (isWindows) Marshal.WriteInt32 (ptr, datasOffset,   pAuthData[i].Length);
      else Marshal.WriteInt64 (ptr, datasOffset,   pAuthData[i].Length);

      Marshal.WriteIntPtr(ptr, datasOffset+(isWindows?4:8), pAuth);
      Marshal.Copy(pAuthData[i], 0, pAuth, pAuthData[i].Length);
      authOffset += pAuthData[i].Length;
    }

    return ptr;
  }

}

}
