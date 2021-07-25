using System.Runtime.InteropServices;

namespace unbound.cryptoki {

public class CK_INFO
{
  public CK_VERSION cryptokiVersion;
  public string manufacturerID;
  public uint flags;
  public string libraryDescription;
  public CK_VERSION libraryVersion;

  internal CK_INFO(NativeWindows native)
  {
    cryptokiVersion = new CK_VERSION(native.cryptokiVersion);
    flags = native.flags;
    libraryVersion = native.libraryVersion;
    libraryDescription = Library.Utf8ToString(native.libraryDescription);
    manufacturerID = Library.Utf8ToString(native.manufacturerID);      
  }

  internal CK_INFO(NativeUnix native)
  {
    cryptokiVersion = new CK_VERSION(native.cryptokiVersion);
    flags = (uint)native.flags;
    libraryVersion = native.libraryVersion;
    libraryDescription = Library.Utf8ToString(native.libraryDescription);
    manufacturerID = Library.Utf8ToString(native.manufacturerID);      
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)] internal struct NativeWindows
  {
    public CK_VERSION.NativeWindows cryptokiVersion;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]      
    public byte[] manufacturerID;
    public uint flags;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]      
    public byte[] libraryDescription;
    public CK_VERSION libraryVersion;
  }

  [StructLayout(LayoutKind.Sequential)] internal struct NativeUnix
  {
    public CK_VERSION.NativeUnix cryptokiVersion;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]      
    public byte[] manufacturerID;
    public ulong flags;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]      
    public byte[] libraryDescription;
    public CK_VERSION libraryVersion;
  }
}

} //namespace unbound.cryptoki
