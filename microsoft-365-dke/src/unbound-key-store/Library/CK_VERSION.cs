using System.Runtime.InteropServices;

namespace unbound.cryptoki {


public struct CK_VERSION
{
  public byte major;
  public byte minor;

  internal CK_VERSION(NativeUnix native)
  {
    major = native.major;
    minor = native.minor;
  }

  internal CK_VERSION(NativeWindows native)
  {
    major = native.major;
    minor = native.minor;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]  internal struct NativeWindows
  {
    public byte major;
    public byte minor;
  }

  [StructLayout(LayoutKind.Sequential)]  internal struct NativeUnix
  {
    public byte major;
    public byte minor;
  }
}

} //namespace unbound.cryptoki