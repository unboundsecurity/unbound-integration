using System.Runtime.InteropServices;

namespace unbound.cryptoki {



public struct CK_MECHANISM_INFO
{
  public int ulMinKeySize;
  public int ulMaxKeySize;
  public uint flags;

  internal CK_MECHANISM_INFO(NativeWindows native)
  {
    ulMinKeySize = native.ulMaxKeySize;
    ulMaxKeySize = native.ulMaxKeySize;
    flags = native.flags;
  }

  internal CK_MECHANISM_INFO(NativeUnix native)
  {
    ulMinKeySize = (int)native.ulMaxKeySize;
    ulMaxKeySize = (int)native.ulMaxKeySize;
    flags = (uint)native.flags;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)] internal struct NativeWindows
  {
    public int ulMinKeySize;
    public int ulMaxKeySize;
    public uint flags;
  };

  [StructLayout(LayoutKind.Sequential)] internal struct NativeUnix
  {
    public ulong ulMinKeySize;
    public ulong ulMaxKeySize;
    public ulong flags;
  };
};

} //namespace unbound.cryptoki