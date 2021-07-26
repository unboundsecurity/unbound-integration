using System;
using System.Runtime.InteropServices;

namespace unbound.cryptoki {

public interface CK_MECHANISM_PARAM
{
  IntPtr ToBin(out int size);  
}

internal class CK_MECHANISM_PARAM_Buffer
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

  internal CK_MECHANISM_PARAM_Buffer(int size, int ext)
  {
    Size = size;
    Ptr = Marshal.AllocCoTaskMem(size+ext);
    BasePtr = Ptr.ToInt64();
    Offset = 0;
    ExtOffset = Offset + size;
  }  
  
  internal IntPtr Ptr { get; }
  internal int Size { get; }
  private long BasePtr;
  private int Offset;
  private int ExtOffset;

  internal void Pack(int value)
  {
    if (isWindows) 
    {
      Marshal.WriteInt32 (Ptr, Offset, value); 
      Offset += 4;
    }
    else 
    {
      Marshal.WriteInt64 (Ptr, Offset, (long)(uint)value); 
      Offset += 8;
    }
  }

  internal void Pack(bool value)
  {
    if (isWindows) 
    {
      Marshal.WriteByte (Ptr, Offset, (byte)(value ? 1 : 0)); 
      Offset += 1;
    }
    else
    {
      Marshal.WriteInt32 (Ptr, Offset, value ? 1 : 0); 
      Offset += 4;
    }
  }

  internal void Pack(uint value)
  {
    Pack((int)value);
  }

  internal void Pack(byte[] a)
  {
    int aLen = a==null ? 0 : a.Length;
    long ptrValue = aLen==0 ? 0 : BasePtr + ExtOffset;
    Marshal.WriteInt64(Ptr, Offset, ptrValue); 
    Offset += 8;

    if (aLen>0)
    {
      Marshal.Copy(a,  0, new IntPtr(BasePtr+ExtOffset),  aLen);
      ExtOffset += aLen;
    }
  }

  internal void Pack(char[] a)
  {
    int aLen = a==null ? 0 : a.Length;
    long ptrValue = aLen==0 ? 0 : BasePtr + ExtOffset;
    Marshal.WriteInt64(Ptr, Offset, ptrValue); 
    Offset += 8;

    if (aLen>0)
    {
      for (int i = 0; i < aLen; i++) Marshal.WriteByte(Ptr, ExtOffset+i, (byte)a[i]);
      Marshal.WriteByte(Ptr, ExtOffset+aLen, 0); // zero terminated
      ExtOffset += aLen+1;
    }
  }
}

public class CK_MECHANISM
{
  public uint mechanism;
  public CK_MECHANISM_PARAM parameter;

  public CK_MECHANISM(uint mechanism, CK_MECHANISM_PARAM parameter=null)
  {
    this.mechanism = mechanism;
    this.parameter = parameter;
  }

  internal class Native
  {
    private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    internal NativeWindows windows;
    internal NativeUnix unix;

    internal Native(CK_MECHANISM mech)
    {
      if (isWindows) windows = new NativeWindows(mech);
      else unix = new NativeUnix(mech);
    }

    internal void Free()
    {
      if (isWindows) windows.Free();
      else unix.Free();
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)] internal struct NativeWindows
  {
    public uint mechanism;
    public IntPtr pParameter;
    public int ulParameterLen;

    internal NativeWindows(CK_MECHANISM mech)
    {
      mechanism = mech.mechanism;
      ulParameterLen = 0;
      pParameter = IntPtr.Zero;
      if (mech.parameter!=null) pParameter = mech.parameter.ToBin(out ulParameterLen);
    }

    internal void Free()
    {
      if (pParameter!=IntPtr.Zero) Marshal.FreeCoTaskMem(pParameter);
      pParameter = IntPtr.Zero;
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  internal struct NativeUnix
  {
    public ulong mechanism;
    public IntPtr pParameter;
    public long ulParameterLen;

    internal NativeUnix(CK_MECHANISM mech)
    {
      mechanism = mech.mechanism;
      ulParameterLen = 0;
      pParameter = IntPtr.Zero;
      if (mech.parameter!=null)
      {
        int paramLen;
        pParameter = mech.parameter.ToBin(out paramLen);
        ulParameterLen = paramLen;
      }
    }

    internal void Free()
    {
      if (pParameter!=IntPtr.Zero) Marshal.FreeCoTaskMem(pParameter);
      pParameter = IntPtr.Zero;
    }
  }

}

} //unbound.cryptoki
