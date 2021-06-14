using System.Runtime.InteropServices;

namespace unbound.cryptoki {

  
public struct CK_SESSION_INFO
{
  public uint slotID;
  public uint state;
  public uint flags;
  public uint ulDeviceError;

  internal CK_SESSION_INFO(NativeWindows native)
  {
    slotID = native.slotID;
    state = native.state;
    flags = native.flags;
    ulDeviceError = native.ulDeviceError;
  }

  internal CK_SESSION_INFO(NativeUnix native)
  {
    slotID = (uint)native.slotID;
    state = (uint)native.state;
    flags = (uint)native.flags;
    ulDeviceError = (uint)native.ulDeviceError;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]  internal struct NativeWindows
  {
    internal uint slotID;
    internal uint state;
    internal uint flags;
    internal uint ulDeviceError;
  }

  [StructLayout(LayoutKind.Sequential)] internal struct NativeUnix
  {
    internal ulong slotID;
    internal ulong state;
    internal ulong flags;
    internal ulong ulDeviceError;
  }
}


} // namespace unbound.cryptoki
