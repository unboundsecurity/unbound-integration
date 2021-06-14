using System.Runtime.InteropServices;

namespace unbound.cryptoki 
{

public class CK_SLOT_INFO
{
  public string slotDescription;
  public string manufacturerID;
  public uint flags;
  public CK_VERSION hardwareVersion;
  public CK_VERSION firmwareVersion;

  internal CK_SLOT_INFO(NativeWindows native)
  {
    flags = native.flags;
    hardwareVersion = new CK_VERSION(native.hardwareVersion);
    firmwareVersion = new CK_VERSION(native.firmwareVersion);
    slotDescription = Library.Utf8ToString(native.slotDescription);
    manufacturerID = Library.Utf8ToString(native.manufacturerID);
  }

  internal CK_SLOT_INFO(NativeUnix native)
  {
    flags = (uint)native.flags;
    hardwareVersion = new CK_VERSION(native.hardwareVersion);
    firmwareVersion = new CK_VERSION(native.firmwareVersion);
    slotDescription = Library.Utf8ToString(native.slotDescription);
    manufacturerID = Library.Utf8ToString(native.manufacturerID);
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)] internal struct NativeWindows
  {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]      
    public byte[] slotDescription;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]      
    public byte[] manufacturerID;

    public uint flags;
    public CK_VERSION.NativeWindows hardwareVersion;
    public CK_VERSION.NativeWindows firmwareVersion;
  }

  [StructLayout(LayoutKind.Sequential)] internal struct NativeUnix
  {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]      
    public byte[] slotDescription;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]      
    public byte[] manufacturerID;

    public ulong flags;
    public CK_VERSION.NativeUnix hardwareVersion;
    public CK_VERSION.NativeUnix firmwareVersion;
  }
}

} // namespace unbound.cryptoki
