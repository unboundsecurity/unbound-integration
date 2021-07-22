using System;
using System.Runtime.InteropServices;
using System.Text;

namespace unbound.cryptoki {

public static partial class Library
{
  private static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

  public static void C_Initialize()
  {
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_Initialize(IntPtr.Zero) :
      LibraryUnix.fl.C_Initialize(IntPtr.Zero)
      , "C_Initialize");
  }
  public static void C_Finalize()
  {
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_Finalize(IntPtr.Zero) :
      LibraryUnix.fl.C_Finalize(IntPtr.Zero)
      , "C_Finalize");
  }

  internal static string Utf8ToString(byte[] utf8)
  {
    int utf8_size = utf8.Length;
    Decoder d = Encoding.UTF8.GetDecoder();

    int charCount = d.GetCharCount(utf8, 0, utf8_size);
    char[] buffer = new char[charCount];
    d.GetChars(utf8, 0, utf8_size, buffer, 0);
    while (charCount > 0 && (buffer[charCount - 1] == ' ' || buffer[charCount - 1] == '\0')) charCount--;
    return new string(buffer, 0, charCount);
  }

  public static CK_INFO C_GetInfo()
  {
    if (isWindows)
    {
      CK_INFO.NativeWindows native = new CK_INFO.NativeWindows();
      CKR_Exception.check(LibraryWindows.fl.C_GetInfo(out native), "C_GetInfo");
      return new CK_INFO(native);
    }
    else
    {
      CK_INFO.NativeUnix native = new CK_INFO.NativeUnix();
      CKR_Exception.check(LibraryUnix.fl.C_GetInfo(out native), "C_GetInfo");
      return new CK_INFO(native);
    }
  }

  public static CK_SLOT_ID[] C_GetSlotList(bool present)
  {
    byte tokenPresent = (byte)(present ? 1 : 0);
    int count = 0;
    long countUnix = 0;
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_GetSlotList(tokenPresent, null, ref count) :
      LibraryUnix.fl.C_GetSlotList(tokenPresent, null, ref countUnix)
      , "C_GetSlotList");

    if (!isWindows) count = (int)countUnix;
    if (count == 0) return new CK_SLOT_ID[0];

    uint[] list = null;
    ulong[] listUnix = null;

    if (isWindows) list = new uint[count];
    else listUnix  = new ulong[count];

    if (count > 0) CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_GetSlotList(tokenPresent, list, ref count):
      LibraryUnix.fl.C_GetSlotList(tokenPresent, listUnix, ref countUnix)
      , "C_GetSlotList");
    
    if (!isWindows) count = (int)countUnix;
    CK_SLOT_ID[] slots = new CK_SLOT_ID[count];
    for (int i=0; i<count; i++) 
    {
      slots[i] = new CK_SLOT_ID(isWindows ? list[i] : (uint)listUnix[i]);
    }
    return slots;
  }

  public static uint[] C_GetMechanismList(CK_SLOT_ID slot)
  {
    int count = 0;
    long countUnix = 0;

    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_GetMechanismList(slot.Id, null, ref count):
      LibraryUnix.fl.C_GetMechanismList(slot.Id, null, ref countUnix)
      , "C_GetMechanismList");
    if (count == 0) return new uint[0];

    if (!isWindows) count = (int)countUnix;
    uint[] list = new uint[count];

    ulong[] listUnix = null;
    if (!isWindows) listUnix  = new ulong[count];

    if (count > 0) 
    {
        CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_GetMechanismList(slot.Id, list, ref count) :
      LibraryUnix.fl.C_GetMechanismList(slot.Id, listUnix, ref countUnix)
      , "C_GetMechanismList");
    }

    if (!isWindows)
    {
      for (int i=0; i<count; i++) list[i] = (uint)listUnix[i];
    }

    return list;
  }

  public static CK_SLOT_INFO C_GetSlotInfo(CK_SLOT_ID slot)
  {
    if (isWindows)
    {
      CK_SLOT_INFO.NativeWindows native = new CK_SLOT_INFO.NativeWindows();
      CKR_Exception.check(LibraryWindows.fl.C_GetSlotInfo(slot.Id, out native), "C_GetSlotInfo");
      return new CK_SLOT_INFO(native);
    }
    else
    {
      CK_SLOT_INFO.NativeUnix native = new CK_SLOT_INFO.NativeUnix();
      CKR_Exception.check(LibraryUnix.fl.C_GetSlotInfo(slot.Id, out native), "C_GetSlotInfo");
      return new CK_SLOT_INFO(native);
    }
  }

  public static CK_TOKEN_INFO C_GetTokenInfo(CK_SLOT_ID slot)
  {
    if (isWindows)
    {
      CK_TOKEN_INFO.NativeWindows native = new CK_TOKEN_INFO.NativeWindows();
      CKR_Exception.check(LibraryWindows.fl.C_GetTokenInfo(slot.Id, out native), "C_GetTokenInfo");
      return new CK_TOKEN_INFO(native);
    }
    else
    {
      CK_TOKEN_INFO.NativeUnix native = new CK_TOKEN_INFO.NativeUnix();
      CKR_Exception.check(LibraryUnix.fl.C_GetTokenInfo(slot.Id, out native), "C_GetTokenInfo");
      return new CK_TOKEN_INFO(native);
    }
  }
  public static CK_MECHANISM_INFO C_GetMechanismInfo(CK_SLOT_ID slot, uint type)
  {
    if (isWindows)
    {
      CK_MECHANISM_INFO.NativeWindows info = new CK_MECHANISM_INFO.NativeWindows();
      CKR_Exception.check(LibraryWindows.fl.C_GetMechanismInfo(slot.Id, type, out info), "C_GetMechanismInfo (" + String.Format("{0:X}", type) + ")");
      return new CK_MECHANISM_INFO(info);
    }
    else
    {
      CK_MECHANISM_INFO.NativeUnix info = new CK_MECHANISM_INFO.NativeUnix();
      CKR_Exception.check(LibraryUnix.fl.C_GetMechanismInfo(slot.Id, type, out info), "C_GetMechanismInfo (" + String.Format("{0:X}", type) + ")");
      return new CK_MECHANISM_INFO(info);  
    }
  }

  public static CK_SESSION_HANDLE C_OpenSession(CK_SLOT_ID slotID, uint flags = CK.CKF_SERIAL_SESSION|CK.CKF_RW_SESSION)
  {
    uint handle = 0;
    ulong handleUnix = 0;
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_OpenSession(slotID.Id, flags, IntPtr.Zero, IntPtr.Zero, out handle) : 
      LibraryUnix.fl.C_OpenSession(slotID.Id, flags, IntPtr.Zero, IntPtr.Zero, out handleUnix)
      , "C_OpenSession (" + slotID + ")");
    return new CK_SESSION_HANDLE(isWindows ? handle : (uint)handleUnix);
  }

  public static void C_CloseSession(CK_SESSION_HANDLE session)
  {
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_CloseSession(session.Handle) : 
      LibraryUnix.fl.C_CloseSession(session.Handle)
      , "C_OpenSession");
  }

  public static void C_CloseAllSessions(CK_SLOT_ID slot)
  {
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_CloseAllSessions(slot.Id):
      LibraryUnix.fl.C_CloseAllSessions(slot.Id)
      , "C_CloseAllSessions");
  }

  public static CK_SESSION_INFO C_GetSessionInfo(CK_SESSION_HANDLE session)
  {
    if (isWindows)
    {
      CK_SESSION_INFO.NativeWindows info = new CK_SESSION_INFO.NativeWindows();
      CKR_Exception.check(LibraryWindows.fl.C_GetSessionInfo(session.Handle, out info), "C_GetSessionInfo");
      return new CK_SESSION_INFO(info);
    }
    else
    {
      CK_SESSION_INFO.NativeUnix info = new CK_SESSION_INFO.NativeUnix();
      CKR_Exception.check(LibraryUnix.fl.C_GetSessionInfo(session.Handle, out info), "C_GetSessionInfo");
      return new CK_SESSION_INFO(info);
    }
  }

  public static void C_Login(CK_SESSION_HANDLE session, uint userType, string pin)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(pin);
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_Login(session.Handle, userType, bytes, (uint)bytes.Length):
      LibraryUnix.fl.C_Login(session.Handle, userType, bytes, (uint)bytes.Length)
      , "C_Login");
  }

  public static void C_Logout(CK_SESSION_HANDLE session)
  {
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_Logout(session.Handle) :
      LibraryUnix.fl.C_Logout(session.Handle)
      , "C_Logout");
  }
  public static void C_DestroyObject(CK_SESSION_HANDLE session, CK_OBJECT_HANDLE obj)
  {
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_DestroyObject(session.Handle, obj.Handle):
      LibraryUnix.fl.C_DestroyObject(session.Handle, obj.Handle)
      , "C_DestroyObject");
  }

  public static void C_SeedRandom(CK_SESSION_HANDLE session, byte[] data)
  {
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_SeedRandom(session.Handle, data, (uint)data.Length):
      LibraryUnix.fl.C_SeedRandom(session.Handle, data, (uint)data.Length)
      , "C_SeedRandom");
  }

  public static byte[] C_GenerateRandom(CK_SESSION_HANDLE session, uint len)
  {
    byte[] dst = new byte[len];
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_GenerateRandom(session.Handle, dst, len) :
      LibraryUnix.fl.C_GenerateRandom(session.Handle, dst, len)
      , "C_GenerateRandom"
      );
    return dst;
  }

    
  public static CK_OBJECT_HANDLE C_CreateObject(CK_SESSION_HANDLE session, CK_ATTRIBUTE[] pTemplate)
  {
    CK_ATTRIBUTE.Native t = CK_ATTRIBUTE.ToNative(pTemplate);
    uint objHandle = 0;
    ulong objHandleUnix = 0;
    try { CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_CreateObject(session.Handle, t.windows, t.Length, out objHandle):
      LibraryUnix.fl.C_CreateObject(session.Handle, t.unix, t.Length, out objHandleUnix)
      , "C_CreateObject"); }
    finally { CK_ATTRIBUTE.Free(t); }
    return new CK_OBJECT_HANDLE(isWindows ? objHandle : (uint)objHandleUnix);
  }

  public static void C_SetAttributeValue(CK_SESSION_HANDLE session, CK_OBJECT_HANDLE obj, CK_ATTRIBUTE[] pTemplate)
  {
    CK_ATTRIBUTE.Native t = CK_ATTRIBUTE.ToNative(pTemplate);
    try { CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_SetAttributeValue(session.Handle, obj.Handle, t.windows, t.Length):
      LibraryUnix.fl.C_SetAttributeValue(session.Handle, obj.Handle, t.unix, t.Length)
      , "C_SetAttributeValue"); }
    finally { CK_ATTRIBUTE.Free(t); }
  }

  public static void C_FindObjectsInit(CK_SESSION_HANDLE session, CK_ATTRIBUTE[] pTemplate)
  {
    CK_ATTRIBUTE.Native t = CK_ATTRIBUTE.ToNative(pTemplate);
    try { CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_FindObjectsInit(session.Handle, t.windows, t.Length):
      LibraryUnix.fl.C_FindObjectsInit(session.Handle, t.unix, t.Length)
      , "C_FindObjectsInit"); }
    finally { CK_ATTRIBUTE.Free(t); }
  }

  public static CK_OBJECT_HANDLE[] C_FindObjects(CK_SESSION_HANDLE session, int ulMaxObjectCount)
  {
    uint[] buf = null;
    ulong[] bufUnix = null;
    
    if (isWindows) buf = new uint[ulMaxObjectCount];
    else bufUnix = new ulong[ulMaxObjectCount];

    int count = 0;
    long countUnix = 0;

    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_FindObjects(session.Handle, buf, ulMaxObjectCount, out count):
      LibraryUnix.fl.C_FindObjects(session.Handle, bufUnix, ulMaxObjectCount, out countUnix)
      , "C_FindObjectsInit");

    if (!isWindows) count = (int)countUnix;
    CK_OBJECT_HANDLE[] res = new CK_OBJECT_HANDLE[count];
    for (int i=0; i<count; i++) res[i] = new CK_OBJECT_HANDLE(isWindows ? buf[i] : (uint)bufUnix[i]); 
    return res;
  }

  public static void C_FindObjectsFinal(CK_SESSION_HANDLE session)
  {
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_FindObjectsFinal(session.Handle):
      LibraryUnix.fl.C_FindObjectsFinal(session.Handle)
      , "C_FindObjectsFinal");
  }

  public static void C_GetAttributeValue(CK_SESSION_HANDLE session, CK_OBJECT_HANDLE obj, CK_ATTRIBUTE[] pTemplate)
  {
    CK_ATTRIBUTE.Native t = null;
    IntPtr buf = IntPtr.Zero;
    try
    {
      bool isKnownSize;
      t = CK_ATTRIBUTE.ToNativeReadSize(pTemplate, out isKnownSize);

      if (!isKnownSize)
      {
        CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_GetAttributeValue(session.Handle, obj.Handle, t.windows, t.Length):
          LibraryUnix.fl.C_GetAttributeValue(session.Handle, obj.Handle, t.unix, t.Length)
          , "C_GetAttributeValue(null)");
      }

      CK_ATTRIBUTE.ToNativeRead(t);
      CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_GetAttributeValue(session.Handle, obj.Handle, t.windows, t.Length):
          LibraryUnix.fl.C_GetAttributeValue(session.Handle, obj.Handle, t.unix, t.Length)
          , "C_GetAttributeValue");
      CK_ATTRIBUTE.FromNative(pTemplate, t);
    }
    finally
    {
      CK_ATTRIBUTE.Free(t);
    }
  }

  public static void C_GenerateKeyPair(CK_SESSION_HANDLE session, CK_MECHANISM mech,
      CK_ATTRIBUTE[] tPub,
      CK_ATTRIBUTE[] tPrv,
      out CK_OBJECT_HANDLE pubKey,
      out CK_OBJECT_HANDLE prvKey)
  {
    CK_ATTRIBUTE.Native tPubRaw = CK_ATTRIBUTE.ToNative(tPub);
    CK_ATTRIBUTE.Native tPrvRaw = CK_ATTRIBUTE.ToNative(tPrv);
    CK_MECHANISM.Native m = new CK_MECHANISM.Native(mech);
    uint pubKeyHandle = 0; 
    uint prvKeyHandle = 0;
    ulong pubKeyHandleUnix = 0; 
    ulong prvKeyHandleUnix = 0;
    try { CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_GenerateKeyPair(session.Handle, m.windows, tPubRaw.windows, tPub.Length, tPrvRaw.windows, tPrv.Length, out pubKeyHandle, out prvKeyHandle):
          LibraryUnix.fl.C_GenerateKeyPair(session.Handle, ref m.unix, tPubRaw.unix, tPub.Length, tPrvRaw.unix, tPrv.Length, out pubKeyHandleUnix, out prvKeyHandleUnix)
          , "C_GenerateKeyPair"); }
    finally { CK_ATTRIBUTE.Free(tPubRaw); CK_ATTRIBUTE.Free(tPrvRaw); m.Free();  }
    pubKey = new CK_OBJECT_HANDLE(isWindows ? pubKeyHandle : (uint)pubKeyHandleUnix);
    prvKey = new CK_OBJECT_HANDLE(isWindows ? prvKeyHandle : (uint)prvKeyHandleUnix);

  }

  public static void C_EncryptInit(CK_SESSION_HANDLE session, CK_MECHANISM mech, CK_OBJECT_HANDLE key)
  {
    CK_MECHANISM.Native m = new CK_MECHANISM.Native(mech);
    try { CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_EncryptInit(session.Handle, m.windows, key.Handle):
          LibraryUnix.fl.C_EncryptInit(session.Handle, ref m.unix, key.Handle)
          , "C_EncryptInit"); }
    finally { m.Free(); }
  }

  public static void C_DecryptInit(CK_SESSION_HANDLE session, CK_MECHANISM mech, CK_OBJECT_HANDLE key)
  {
    CK_MECHANISM.Native m = new CK_MECHANISM.Native(mech);
    try { CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_DecryptInit(session.Handle, m.windows, key.Handle):
          LibraryUnix.fl.C_DecryptInit(session.Handle, ref m.unix, key.Handle)
          , "C_DecryptInit"); }
    finally { m.Free(); }
  }

  public static void C_SignInit(CK_SESSION_HANDLE session, CK_MECHANISM mech, CK_OBJECT_HANDLE key)
  {
    CK_MECHANISM.Native m = new CK_MECHANISM.Native(mech);
    try { CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_SignInit(session.Handle, m.windows, key.Handle):
          LibraryUnix.fl.C_SignInit(session.Handle, ref m.unix, key.Handle)
          , "C_SignInit"); }
    finally { m.Free(); }
  }

  public static void C_VerifyInit(CK_SESSION_HANDLE session, CK_MECHANISM mech, CK_OBJECT_HANDLE key)
  {
    CK_MECHANISM.Native m = new CK_MECHANISM.Native(mech);
    try { CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_VerifyInit(session.Handle, m.windows, key.Handle):
          LibraryUnix.fl.C_VerifyInit(session.Handle, ref m.unix, key.Handle)
          , "C_VerifyInit"); }
    finally { m.Free(); }
  }

  public static void C_DigestInit(CK_SESSION_HANDLE session, CK_MECHANISM mech)
  {
    CK_MECHANISM.Native m = new CK_MECHANISM.Native(mech);
    try { CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_DigestInit(session.Handle, m.windows):
          LibraryUnix.fl.C_DigestInit(session.Handle, ref m.unix)
          , "C_DigestInit"); }
    finally { m.Free(); }
  }

  public static int C_EncryptUpdate(CK_SESSION_HANDLE session, byte[] input,  byte[] output)
  {
    int inSize = input == null ? 0 : input.Length;
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_EncryptUpdate(session.Handle, input, inSize, output, ref outSize):
          LibraryUnix.fl.C_EncryptUpdate(session.Handle, input, inSize, output, ref outSizeUnix)
          , "C_EncryptUpdate");
    return isWindows ? outSize : (int)outSizeUnix;
  }

  public static int C_EncryptFinal(CK_SESSION_HANDLE session, byte[] output)
  {
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_EncryptFinal(session.Handle, output, ref outSize):
          LibraryUnix.fl.C_EncryptFinal(session.Handle, output, ref outSizeUnix)
          , "C_EncryptFinal");
    return isWindows ? outSize : (int)outSizeUnix;
  }

  public static int C_Encrypt(CK_SESSION_HANDLE session, byte[] input, byte[] output)
  {
    int inSize = input == null ? 0 : input.Length;
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_Encrypt(session.Handle, input, inSize, output, ref outSize):
          LibraryUnix.fl.C_Encrypt(session.Handle, input, inSize, output, ref outSizeUnix)
          , "C_Encrypt");
    return isWindows ? outSize : (int)outSizeUnix;
  }

  private static byte[] SubArray(byte[] src, int length)
  {
    if (src.Length==length) return src;
    byte[] result = new byte[length];
    Array.Copy(src, 0, result, 0, length);
    return result;
  }

  public static byte[] C_Encrypt(CK_SESSION_HANDLE session, byte[] input)
  {
    int outSize = C_Encrypt(session, input, null);
    byte[] output = new byte[outSize];
    outSize = C_Encrypt(session, input, output);
    return SubArray(output, outSize);
  }

  public static int C_DecryptUpdate(CK_SESSION_HANDLE session, byte[] input, byte[] output)
  {
    int inSize = input == null ? 0 : input.Length;
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_DecryptUpdate(session.Handle, input, inSize, output, ref outSize):
          LibraryUnix.fl.C_DecryptUpdate(session.Handle, input, inSize, output, ref outSizeUnix)
          , "C_DecryptUpdate");
    return isWindows ? outSize : (int)outSizeUnix;
  }

  public static int C_DecryptFinal(CK_SESSION_HANDLE session, byte[] output)
  {
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_DecryptFinal(session.Handle, output, ref outSize):
          LibraryUnix.fl.C_DecryptFinal(session.Handle, output, ref outSizeUnix)
          , "C_DecryptFinal");
    return isWindows ? outSize : (int)outSizeUnix;
  }
  public static int C_Decrypt(CK_SESSION_HANDLE session, byte[] input, byte[] output)
  {
    int inSize = input == null ? 0 : input.Length;
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_Decrypt(session.Handle, input, inSize, output, ref outSize):
          LibraryUnix.fl.C_Decrypt(session.Handle, input, inSize, output, ref outSizeUnix)
          , "C_Decrypt");
    return isWindows ? outSize : (int)outSizeUnix;
  }

  public static byte[] C_Decrypt(CK_SESSION_HANDLE session, byte[] input)
  {
    int outSize = C_Decrypt(session, input, null);
    byte[] output = new byte[outSize];
    outSize = C_Decrypt(session, input, output);
    return SubArray(output, outSize);
  }

  public static void C_SignUpdate(CK_SESSION_HANDLE session, byte[] input)
  {
    int inSize = input == null ? 0 : input.Length;
    CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_SignUpdate(session.Handle, input, inSize):
          LibraryUnix.fl.C_SignUpdate(session.Handle, input, inSize)
          , "C_SignUpdate");
  }

  public static int C_SignFinal(CK_SESSION_HANDLE session, byte[] output)
  {
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_SignFinal(session.Handle, output, ref outSize):
          LibraryUnix.fl.C_SignFinal(session.Handle, output, ref outSizeUnix)
          , "C_SignFinal");
    return isWindows ? outSize : (int)outSizeUnix;
  }

  public static int C_Sign(CK_SESSION_HANDLE session, byte[] input, byte[] output)
  {
    int inSize = input == null ? 0 : input.Length;
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    CKR_Exception.check(isWindows ? 
          LibraryWindows.fl.C_Sign(session.Handle, input, inSize, output, ref outSize):
          LibraryUnix.fl.C_Sign(session.Handle, input, inSize, output, ref outSizeUnix)
          , "C_Sign");
    return isWindows ? outSize : (int)outSizeUnix;
  }

  public static byte[] C_Sign(CK_SESSION_HANDLE session, byte[] input)
  {
    int outSize = C_Sign(session, input, null);
    byte[] output = new byte[outSize];
    outSize = C_Sign(session, input, output);
    return SubArray(output, outSize);
  }

  public static void C_VerifyUpdate(CK_SESSION_HANDLE session, byte[] input)
  {
    int inSize = input == null ? 0 : input.Length;
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_VerifyUpdate(session.Handle, input, inSize):
      LibraryUnix.fl.C_VerifyUpdate(session.Handle, input, inSize)
      , "C_VerifyUpdate");
  }

  public static void C_VerifyFinal(CK_SESSION_HANDLE session, byte[] signature)
  {
    int sigSize = signature == null ? 0 : signature.Length;
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_VerifyFinal(session.Handle, signature, sigSize):
      LibraryUnix.fl.C_VerifyFinal(session.Handle, signature, sigSize)
      , "C_VerifyFinal");
  }

  public static void C_Verify(CK_SESSION_HANDLE session, byte[] input, byte[] signature)
  {
    int inSize = input == null ? 0 : input.Length;
    int sigSize = signature == null ? 0 : signature.Length;
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_Verify(session.Handle, input, inSize, signature, sigSize):
      LibraryUnix.fl.C_Verify(session.Handle, input, inSize, signature, sigSize)
      , "C_Verify");
  }

  public static void C_DigestUpdate(CK_SESSION_HANDLE session, byte[] input)
  {
    int inSize = input == null ? 0 : input.Length;
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_DigestUpdate(session.Handle, input, inSize):
      LibraryUnix.fl.C_DigestUpdate(session.Handle, input, inSize)
      , "C_DigestUpdate");
  }

  public static int C_DigestFinal(CK_SESSION_HANDLE session, byte[] output)
  {
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_DigestFinal(session.Handle, output, ref outSize):
      LibraryUnix.fl.C_DigestFinal(session.Handle, output, ref outSizeUnix)
      , "C_DigestFinal");
    return isWindows ? outSize : (int)outSizeUnix;
  }

  public static int C_Digest(CK_SESSION_HANDLE session, byte[] input, byte[] output)
  {
    int inSize = input == null ? 0 : input.Length;
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_Digest(session.Handle, input, inSize, output, ref outSize):
      LibraryUnix.fl.C_Digest(session.Handle, input, inSize, output, ref outSizeUnix)
      , "C_Digest");
    return isWindows ? outSize : (int)outSizeUnix;
  }

  public static byte[] C_Digest(CK_SESSION_HANDLE session, byte[] input)
  {
    int outSize = C_Digest(session, input, null);
    byte[] output = new byte[outSize];
    outSize = C_Digest(session, input, output);
    return SubArray(output, outSize);
  }

  public static void C_DigestKey(CK_SESSION_HANDLE session, CK_OBJECT_HANDLE key)
  {
    CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_DigestKey(session.Handle, key.Handle):
      LibraryUnix.fl.C_DigestKey(session.Handle, key.Handle)
      , "C_DigestKey");
  }

  public static int C_WrapKey(CK_SESSION_HANDLE session, CK_MECHANISM mech, CK_OBJECT_HANDLE key, CK_OBJECT_HANDLE wrappedKey, byte[] output)
  {
    CK_MECHANISM.Native m = new CK_MECHANISM.Native(mech);
    int outSize = output == null ? 0 : output.Length;
    long outSizeUnix = output == null ? 0 : output.Length;

    try { CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_WrapKey(session.Handle, m.windows, key.Handle, wrappedKey.Handle, output, ref outSize):
      LibraryUnix.fl.C_WrapKey(session.Handle, ref m.unix, key.Handle, wrappedKey.Handle, output, ref outSizeUnix)
      , "C_WrapKey"); }
    finally { m.Free(); }
    return isWindows ? outSize : (int)outSizeUnix;
  }

  public static byte[] C_WrapKey(CK_SESSION_HANDLE session, CK_MECHANISM mech, CK_OBJECT_HANDLE key, CK_OBJECT_HANDLE wrappedKey)
  {
    int outSize = C_WrapKey(session, mech, key, wrappedKey, null);
    byte[] output = new byte[outSize];
    outSize = C_WrapKey(session, mech, key, wrappedKey, output);
    return SubArray(output, outSize);
  }

  public static CK_OBJECT_HANDLE C_UnwrapKey(CK_SESSION_HANDLE session, CK_MECHANISM mech, CK_OBJECT_HANDLE key, byte[] input, CK_ATTRIBUTE[] pTemplate)
  {
    CK_MECHANISM.Native m = new CK_MECHANISM.Native(mech);
    CK_ATTRIBUTE.Native t = CK_ATTRIBUTE.ToNative(pTemplate);
    int inSize = input == null ? 0 : input.Length;
    uint unwrappedKeyHandle = 0;
    ulong unwrappedKeyHandleUnix = 0;
    try { CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_UnwrapKey(session.Handle, m.windows, key.Handle, input, inSize, t.windows, t.Length, out unwrappedKeyHandle):
      LibraryUnix.fl.C_UnwrapKey(session.Handle, ref m.unix, key.Handle, input, inSize, t.unix, t.Length, out unwrappedKeyHandleUnix)
      , "C_UnwrapKey"); }
    finally { m.Free(); CK_ATTRIBUTE.Free(t); }
    return new CK_OBJECT_HANDLE(isWindows ? unwrappedKeyHandle : (uint)unwrappedKeyHandleUnix);
  }

  public static CK_OBJECT_HANDLE C_DeriveKey(CK_SESSION_HANDLE session, CK_MECHANISM mech, CK_OBJECT_HANDLE key, CK_ATTRIBUTE[] pTemplate)
  {
    CK_MECHANISM.Native m = new CK_MECHANISM.Native(mech);
    CK_ATTRIBUTE.Native t = CK_ATTRIBUTE.ToNative(pTemplate);
    uint derivedKeyHandle = 0;
    ulong derivedKeyHandleUnix = 0;

    try { CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_DeriveKey(session.Handle, m.windows, key.Handle, t.windows, t.Length, out derivedKeyHandle):
      LibraryUnix.fl.C_DeriveKey(session.Handle, ref m.unix, key.Handle, t.unix, t.Length, out derivedKeyHandleUnix)
      , "C_DeriveKey"); }
    finally { m.Free(); CK_ATTRIBUTE.Free(t); }
    return new CK_OBJECT_HANDLE(isWindows ? derivedKeyHandle : (uint)derivedKeyHandleUnix);
  }

  public static CK_OBJECT_HANDLE C_GenerateKey(CK_SESSION_HANDLE session, CK_MECHANISM mech, CK_ATTRIBUTE[] pTemplate)
  {
    CK_MECHANISM.Native m = new CK_MECHANISM.Native(mech);
    CK_ATTRIBUTE.Native t = CK_ATTRIBUTE.ToNative(pTemplate);
    uint keyHandle = 0;
    ulong keyHandleUnix = 0;

    try { CKR_Exception.check(isWindows ? 
      LibraryWindows.fl.C_GenerateKey(session.Handle, m.windows, t.windows, t.Length, out keyHandle):
      LibraryUnix.fl.C_GenerateKey(session.Handle, ref m.unix, t.unix, t.Length, out keyHandleUnix)
      , "C_GenerateKey"); }
    finally { m.Free(); CK_ATTRIBUTE.Free(t); }
    return new CK_OBJECT_HANDLE(isWindows ? keyHandle : (uint)keyHandleUnix);
  }
}

} // namespace unbound.cryptoki
