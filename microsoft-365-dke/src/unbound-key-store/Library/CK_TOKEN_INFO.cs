using System.Runtime.InteropServices;


namespace unbound.cryptoki {

public class CK_TOKEN_INFO
{
  public string label;
  public string manufacturerID;
  public string model;
  public string serialNumber;
  public uint flags;
  public int ulMaxSessionCount;
  public int ulSessionCount;
  public int ulMaxRwSessionCount;
  public int ulRwSessionCount;
  public int ulMaxPinLen;
  public int ulMinPinLen;
  public int ulTotalPublicMemory;
  public int ulFreePublicMemory;
  public int ulTotalPrivateMemory;
  public int ulFreePrivateMemory;
  public CK_VERSION hardwareVersion;
  public CK_VERSION firmwareVersion;
  public string utcTime;

  internal CK_TOKEN_INFO(NativeWindows native)
  {
    firmwareVersion = new CK_VERSION(native.firmwareVersion);
    flags = native.flags;
    hardwareVersion = new CK_VERSION(native.hardwareVersion);
    label = Library.Utf8ToString(native.label);
    manufacturerID = Library.Utf8ToString(native.manufacturerID);
    model = Library.Utf8ToString(native.model);
    serialNumber = Library.Utf8ToString(native.serialNumber);
    ulFreePrivateMemory = native.ulFreePrivateMemory;
    ulFreePublicMemory = native.ulFreePublicMemory;
    ulMaxPinLen = native.ulMaxPinLen;
    ulMaxRwSessionCount = native.ulMaxRwSessionCount;
    ulMaxSessionCount = native.ulMaxSessionCount;
    ulMinPinLen = native.ulMinPinLen;
    ulRwSessionCount = native.ulRwSessionCount;
    ulSessionCount = native.ulSessionCount;
    ulTotalPrivateMemory = native.ulTotalPrivateMemory;
    ulTotalPublicMemory = native.ulTotalPublicMemory;
    utcTime = Library.Utf8ToString(native.utcTime);
  }

  internal CK_TOKEN_INFO(NativeUnix native)
  {
    firmwareVersion = new CK_VERSION(native.firmwareVersion);
    flags = (uint)native.flags;
    hardwareVersion = new CK_VERSION(native.hardwareVersion);
    label = Library.Utf8ToString(native.label);
    manufacturerID = Library.Utf8ToString(native.manufacturerID);
    model = Library.Utf8ToString(native.model);
    serialNumber = Library.Utf8ToString(native.serialNumber);
    ulFreePrivateMemory = (int)native.ulFreePrivateMemory;
    ulFreePublicMemory = (int)native.ulFreePublicMemory;
    ulMaxPinLen = (int)native.ulMaxPinLen;
    ulMaxRwSessionCount = (int)native.ulMaxRwSessionCount;
    ulMaxSessionCount = (int)native.ulMaxSessionCount;
    ulMinPinLen = (int)native.ulMinPinLen;
    ulRwSessionCount = (int)native.ulRwSessionCount;
    ulSessionCount = (int)native.ulSessionCount;
    ulTotalPrivateMemory = (int)native.ulTotalPrivateMemory;
    ulTotalPublicMemory = (int)native.ulTotalPublicMemory;
    utcTime = Library.Utf8ToString(native.utcTime);
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)] internal struct NativeWindows
  {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]      
    public byte[] label;
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]      
    public byte[] manufacturerID;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]         
    public byte[] model;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]          
    public byte[] serialNumber;

    public uint flags;
    public int ulMaxSessionCount;
    public int ulSessionCount;
    public int ulMaxRwSessionCount;
    public int ulRwSessionCount;
    public int ulMaxPinLen;
    public int ulMinPinLen;
    public int ulTotalPublicMemory;
    public int ulFreePublicMemory;
    public int ulTotalPrivateMemory;
    public int ulFreePrivateMemory;
    public CK_VERSION.NativeWindows hardwareVersion;
    public CK_VERSION.NativeWindows firmwareVersion;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]          
    public byte[] utcTime;
  }

  [StructLayout(LayoutKind.Sequential)] internal struct NativeUnix
  {
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]      
    public byte[] label;
    
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]      
    public byte[] manufacturerID;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]         
    public byte[] model;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]          
    public byte[] serialNumber;

    public ulong flags;
    public long ulMaxSessionCount;
    public long ulSessionCount;
    public long ulMaxRwSessionCount;
    public long ulRwSessionCount;
    public long ulMaxPinLen;
    public long ulMinPinLen;
    public long ulTotalPublicMemory;
    public long ulFreePublicMemory;
    public long ulTotalPrivateMemory;
    public long ulFreePrivateMemory;
    public CK_VERSION.NativeUnix hardwareVersion;
    public CK_VERSION.NativeUnix firmwareVersion;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]          
    public byte[] utcTime;
  }

}

} // namespace unbound.cryptoki