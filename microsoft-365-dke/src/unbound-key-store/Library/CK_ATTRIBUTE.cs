using System;
using System.Runtime.InteropServices;
using System.Text;


namespace unbound.cryptoki {

public class CK_ATTRIBUTE
{
  public uint type;
  public object pValue;

  private enum Type
  {
    Bytes = 0,
    Bool = 1,
    Int = 2,
    Str = 3,
    Date = 4,
    Long = 5,
  }

  public CK_ATTRIBUTE(uint type)
  {
    this.type = type;
    this.pValue = null;
  }

  public CK_ATTRIBUTE(uint type, bool value)
  {
    this.type = type;
    pValue = value;
  }

  public CK_ATTRIBUTE(uint type, int value)
  {
    this.type = type;
    pValue = (UInt32)value;
  }

  public CK_ATTRIBUTE(uint type, uint value)
  {
    this.type = type;
    pValue = (UInt32)value;
  }

  public CK_ATTRIBUTE(uint type, ulong value)
  {
    this.type = type;
    pValue = (UInt64)value;
  }

  public CK_ATTRIBUTE(uint type, byte[] value)
  {
    this.type = type;
    pValue = value;
  }
  public CK_ATTRIBUTE(uint type, string value)
  {
    this.type = type;
    pValue = value;
  }

  private static Type GetAttrMode(uint attr)
  {
    switch (attr)
    {
      case CK.DYCKA_UID:
      case CK.DYCKA_REPLACED_UID:
      case CK.DYCKA_ECDSA_BIP_PARENT_UID:
      case CK.DYCKA_PRIVATE_KEY_UID:
        return Type.Long;

      case CK.CKA_TOKEN:
      case CK.CKA_PRIVATE:
      case CK.CKA_MODIFIABLE:
      case CK.CKA_DERIVE:
      case CK.CKA_LOCAL:
      case CK.CKA_ENCRYPT:
      case CK.CKA_VERIFY:
      case CK.CKA_VERIFY_RECOVER:
      case CK.CKA_WRAP:
      case CK.CKA_SENSITIVE:
      case CK.CKA_SECONDARY_AUTH:
      case CK.CKA_DECRYPT:
      case CK.CKA_SIGN:
      case CK.CKA_SIGN_RECOVER:
      case CK.CKA_UNWRAP:
      case CK.CKA_EXTRACTABLE:
      case CK.CKA_ALWAYS_SENSITIVE:
      case CK.CKA_NEVER_EXTRACTABLE:
      case CK.CKA_TRUSTED:
      case CK.CKA_WRAP_WITH_TRUSTED:
      case CK.CKA_ALWAYS_AUTHENTICATE:
      case CK.DYCKA_ECDSA_BIP_HARDENED:
      case CK.DYCKA_DISTRIBUTED_KEY_GENERATION:
        return Type.Bool;

      case CK.CKA_CLASS:
      case CK.CKA_KEY_TYPE:
      case CK.CKA_CERTIFICATE_TYPE:
      case CK.CKA_MODULUS_BITS:
      case CK.CKA_VALUE_BITS:
      case CK.CKA_VALUE_LEN:
      case CK.CKA_KEY_GEN_MECHANISM:
      case CK.CKA_CERTIFICATE_CATEGORY:
      case CK.DYCKA_ECDSA_BIP_LEVEL:
      case CK.DYCKA_ECDSA_BIP_CHILD_NUMBER:
      case CK.DYCKA_ECDSA_BIP_PARENT_FINGERPRINT:
      case CK.DYCKA_STATE:
        return Type.Int;

      case CK.CKA_START_DATE:
      case CK.CKA_END_DATE:
        return Type.Date;

      case CK.CKA_LABEL:
        return Type.Str;
    }
    return Type.Bytes;
  }

  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
  
  private int GetBufSize()
  {
    switch (GetAttrMode(type))
    {
      case Type.Bool: return 1;
      case Type.Int: return isWindows ? 4 : 8;
      case Type.Date: return 8;
      case Type.Long: return 8;
      case Type.Str: return (pValue == null) ? 0 : Encoding.UTF8.GetByteCount((string)pValue);
    }
    if (pValue == null) return 0;
    return ((byte[])pValue).Length;
  }
  private bool IsKnownSize()
  {
    switch (GetAttrMode(type))
    {
      case Type.Bool: return true;
      case Type.Int: return true;
      case Type.Date: return true;
      case Type.Long: return true;
    }
    return false;
  }

  private static ulong SwapBytes(ulong value)
  {
    ulong uvalue = value;
    ulong swapped =
          ((0x00000000000000FF) & (uvalue >> 56)
          | (0x000000000000FF00) & (uvalue >> 40)
          | (0x0000000000FF0000) & (uvalue >> 24)
          | (0x00000000FF000000) & (uvalue >> 8)
          | (0x000000FF00000000) & (uvalue << 8)
          | (0x0000FF0000000000) & (uvalue << 24)
          | (0x00FF000000000000) & (uvalue << 40)
          | (0xFF00000000000000) & (uvalue << 56));
    return swapped;
  }

  private void Save(IntPtr dst)
  {
    if (pValue == null) return;

    byte[] bytes;
    switch (GetAttrMode(type))
    {
      case Type.Bool:
        Marshal.WriteByte(dst, (byte)(((bool)(Boolean)pValue) ? 1 : 0));
        break;

      case Type.Int:
        if (pValue is Int32)
        {
          if (isWindows) Marshal.WriteInt32(dst, (int)(Int32)pValue);
          else Marshal.WriteInt64(dst, (long)(ulong)(Int32)pValue);
        }
        else if (pValue is UInt32)
        {
          if (isWindows) Marshal.WriteInt32(dst, (int)(UInt32)pValue);
          else Marshal.WriteInt64(dst, (long)(ulong)(UInt32)pValue);
        }
        break;

      case Type.Date:
        Marshal.WriteByte(dst, 0, (byte)('0' + ((DateTime)pValue).Year / 1000));
        Marshal.WriteByte(dst, 1, (byte)('0' + (((DateTime)pValue).Year % 1000) / 100));
        Marshal.WriteByte(dst, 2, (byte)('0' + (((DateTime)pValue).Year % 100) / 10));
        Marshal.WriteByte(dst, 3, (byte)('0' + ((DateTime)pValue).Year % 10));
        Marshal.WriteByte(dst, 4, (byte)('0' + ((DateTime)pValue).Month / 10));
        Marshal.WriteByte(dst, 5, (byte)('0' + ((DateTime)pValue).Month % 10));
        Marshal.WriteByte(dst, 6, (byte)('0' + ((DateTime)pValue).Day / 10));
        Marshal.WriteByte(dst, 7, (byte)('0' + ((DateTime)pValue).Day % 10));
        break;

      case Type.Long:
        Marshal.WriteInt64(dst, (long)SwapBytes((UInt64)pValue));
        break;

      case Type.Str:
        bytes = Encoding.UTF8.GetBytes((string)pValue);
        Marshal.Copy(bytes, 0, dst, bytes.Length);
        break;

      default:
        bytes = (byte[])pValue;
        Marshal.Copy(bytes, 0, dst, bytes.Length);
        break;
    }
  }

  private void Load(IntPtr src, int len)
  {
    switch (GetAttrMode(type))
    {
      case Type.Bool:
        pValue = Marshal.ReadByte(src) == 0 ? false : true;
        break;

      case Type.Int:
        if (isWindows) pValue = Marshal.ReadInt32(src);
        else pValue = (int)Marshal.ReadInt64(src);
        break;

      case Type.Date:
        pValue = new DateTime(
              (Marshal.ReadByte(src, 0) - '0') * 1000 + // year
              (Marshal.ReadByte(src, 1) - '0') * 100 +
              (Marshal.ReadByte(src, 2) - '0') * 10 +
              (Marshal.ReadByte(src, 3) - '0'),
              (Marshal.ReadByte(src, 4) - '0') * 10 +   // month
              (Marshal.ReadByte(src, 5) - '0'),
              (Marshal.ReadByte(src, 6) - '0') * 10 +   // day
              (Marshal.ReadByte(src, 7) - '0'));
        break;

      case Type.Long:
        pValue = (long)SwapBytes((UInt64)Marshal.ReadInt64(src));
        break;

      case Type.Str:
        pValue = Marshal.PtrToStringAuto(src, len);
        break;

      default:
        pValue = new byte[len];
        Marshal.Copy(src, (byte[])pValue, 0, len);
        break;
    }
  }

  internal class Native
  {
    private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    internal NativeWindows[] windows;
    internal NativeUnix[] unix;
    internal int Length { get { return isWindows ? windows.Length : unix.Length; } }
  }

  internal static Native ToNative(CK_ATTRIBUTE[] pTemplate)
  {
    int bufSize = 0;
    for (int i = 0; i < pTemplate.Length; i++)
    {
      bufSize += pTemplate[i].GetBufSize();
    }
    Native native = new Native();

    if (isWindows) native.windows = new NativeWindows[pTemplate.Length];
    else           native.unix    = new NativeUnix[pTemplate.Length];

    IntPtr buf = bufSize == 0 ? IntPtr.Zero : Marshal.AllocCoTaskMem(bufSize);
    int offset = 0;

    for (int i = 0; i < pTemplate.Length; i++)
    {
      if (isWindows) native.windows[i].type = pTemplate[i].type;
      else           native.unix[i].type    = pTemplate[i].type;

      int len = pTemplate[i].GetBufSize();
      
      if (isWindows) native.windows[i].ulValueLen = len;
      else           native.unix[i].ulValueLen    = len;

      if (isWindows) native.windows[i].pValue = IntPtr.Zero;
      else           native.unix[i].pValue    = IntPtr.Zero;

      if (len > 0)
      {
        IntPtr pValue = new IntPtr(buf.ToInt64() + offset);
        if (isWindows) native.windows[i].pValue = pValue;
        else           native.unix[i].pValue    = pValue;

        pTemplate[i].Save(pValue);
        offset += len;
      }
    }
    return native;
  }

  internal static Native ToNativeReadSize(CK_ATTRIBUTE[] pTemplate, out bool isKnownSize)
  {
    Native native = new Native();      
    if (isWindows) native.windows = new NativeWindows[pTemplate.Length];
    else           native.unix    = new NativeUnix[pTemplate.Length];

    isKnownSize = true;

    for (int i = 0; i < pTemplate.Length; i++)
    {
      if (isWindows)  native.windows[i].type = pTemplate[i].type;
      else            native.unix[i].type    = pTemplate[i].type;

      bool known = pTemplate[i].IsKnownSize();
      isKnownSize &= known;
      if (!known) continue;
      
      int bufSize = pTemplate[i].GetBufSize();
      if (isWindows)  native.windows[i].ulValueLen = bufSize;
      else            native.unix[i].ulValueLen = bufSize;
    }

    return native;
  }

  internal static void FromNative(CK_ATTRIBUTE[] pTemplate, Native native)
  {
    for (int i = 0; i < pTemplate.Length; i++)
    {
      pTemplate[i].Load(
        isWindows ? native.windows[i].pValue     : native.unix[i].pValue,
        isWindows ? native.windows[i].ulValueLen : (int)native.unix[i].ulValueLen);
    }
  }

  internal static void ToNativeRead(Native native)
  {
    int bufSize = 0;
    for (int i = 0; i < native.Length; i++) 
    {
      bufSize += isWindows ? native.windows[i].ulValueLen : (int)native.unix[i].ulValueLen;
    }
    IntPtr buf = Marshal.AllocCoTaskMem((int)bufSize);
    
    int offset = 0;
    for (int i = 0; i < native.Length; i++)
    {
      IntPtr pValue = new IntPtr(buf.ToInt64() + offset);
      
      if (isWindows) native.windows[i].pValue = pValue;
      else native.unix[i].pValue = pValue;

      offset += isWindows ? native.windows[i].ulValueLen : (int)native.unix[i].ulValueLen;
    }
  }
  
  internal static void Free(Native native)
  {
    if (native==null) return;

    if (isWindows)
    {
      if (native.windows==null) return;
    }
    else
    {
      if (native.unix==null) return;
    }

    bool deallocated = false;
    for (int i = 0; i < native.Length; i++)
    {
      IntPtr pValue = isWindows ? native.windows[i].pValue : native.unix[i].pValue;
      if (pValue != IntPtr.Zero)
      {
        if (!deallocated) Marshal.FreeCoTaskMem(pValue);
        deallocated = true;
      }
      if (isWindows) native.windows[i].pValue = IntPtr.Zero;
      else           native.unix[i].pValue    = IntPtr.Zero;
    }
  }


  [StructLayout(LayoutKind.Sequential, Pack = 1)] internal struct NativeWindows
  {
    public uint type;
    public IntPtr pValue;
    public int ulValueLen;
  }
  [StructLayout(LayoutKind.Sequential)] internal struct NativeUnix
  {
    public ulong type;
    public IntPtr pValue;
    public long ulValueLen;
  }
}

}
