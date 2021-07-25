using System;
using System.Runtime.InteropServices;
using System.Text;

namespace unbound.cryptoki 
{

public static partial class LibraryUnix
{
  const int RTLD_NOW = 2;
  [DllImport("libdl.so")]
  private static extern IntPtr dlopen(String fileName, int flags);
  [DllImport("libdl.so")]
  private static extern IntPtr dlsym(IntPtr handle, String symbol);


  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GetFunctionList(out IntPtr funcList);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_Initialize(IntPtr dummy);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_Finalize(IntPtr dummy);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GetInfo(out CK_INFO.NativeUnix info);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GetSlotList(byte present,
    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] ulong[] listPtr, ref long count);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GetSlotInfo(uint slot, out CK_SLOT_INFO.NativeUnix info);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GetTokenInfo(uint slot, out CK_TOKEN_INFO.NativeUnix info);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GetMechanismList(uint slot,
    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] ulong[] listPtr, ref long count);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GetMechanismInfo(uint slot, uint mechanism, out CK_MECHANISM_INFO.NativeUnix info);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_OpenSession(uint slot, uint flags,
    IntPtr app, IntPtr notify, out ulong session);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GetSessionInfo(uint session, out CK_SESSION_INFO.NativeUnix info);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_CloseSession(uint session);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_CloseAllSessions(uint slot);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_Login(uint session, uint type,
    [MarshalAs(UnmanagedType.LPArray)] byte[] pin, uint pinLen);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_Logout(uint session);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_DestroyObject(uint session, uint obj);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_CreateObject(uint session,
    [MarshalAs(UnmanagedType.LPArray)] CK_ATTRIBUTE.NativeUnix[] t, int tLen, out ulong obj);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_SetAttributeValue(uint session, uint obj,
    [MarshalAs(UnmanagedType.LPArray)] CK_ATTRIBUTE.NativeUnix[] t, int tLen);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GetAttributeValue(uint session, uint obj,
    [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] CK_ATTRIBUTE.NativeUnix[] t, int tLen);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_FindObjectsInit(uint session,
    [MarshalAs(UnmanagedType.LPArray)] CK_ATTRIBUTE.NativeUnix[] t, int tLen);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_FindObjects(uint session,
    [MarshalAs(UnmanagedType.LPArray)] ulong[] listPtr, int maxCount, out long count);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_FindObjectsFinal(uint session);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_SeedRandom(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] ptr, uint size);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GenerateRandom(uint session,
    [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] ptr, uint size);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GenerateKeyPair(uint session, ref CK_MECHANISM.NativeUnix mech,
    [MarshalAs(UnmanagedType.LPArray)] CK_ATTRIBUTE.NativeUnix[] tPub, int tPubLen,
    [MarshalAs(UnmanagedType.LPArray)] CK_ATTRIBUTE.NativeUnix[] tPrv, int tPrvLen, out ulong pubKey, out ulong prvKey);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_WrapKey(uint session, ref CK_MECHANISM.NativeUnix mech, uint wrappingKey, uint key,
    [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_UnwrapKey(uint session, ref CK_MECHANISM.NativeUnix mech, uint unwrappingKey,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize,
    [MarshalAs(UnmanagedType.LPArray)] CK_ATTRIBUTE.NativeUnix[] t, int tLen, out ulong key);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_DeriveKey(uint session, ref CK_MECHANISM.NativeUnix mech, uint baseKey,
    [MarshalAs(UnmanagedType.LPArray)] CK_ATTRIBUTE.NativeUnix[] t, int tLen, out ulong key);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_GenerateKey(ulong session, ref CK_MECHANISM.NativeUnix mech,
    [MarshalAs(UnmanagedType.LPArray)] CK_ATTRIBUTE.NativeUnix[] t, int tLen, out ulong key);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_EncryptInit(uint session, ref CK_MECHANISM.NativeUnix mech, uint key);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_EncryptUpdate(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_EncryptFinal(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_Encrypt(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_DecryptInit(uint session, ref CK_MECHANISM.NativeUnix mech, uint key);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_DecryptUpdate(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_DecryptFinal(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_Decrypt(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_SignInit(uint session, ref CK_MECHANISM.NativeUnix mech, uint key);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_SignUpdate(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_SignFinal(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_Sign(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_VerifyInit(uint session, ref CK_MECHANISM.NativeUnix mech, uint key);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_VerifyUpdate(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_VerifyFinal(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] signature, int signatureSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_Verify(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize, [MarshalAs(UnmanagedType.LPArray)] byte[] signature, int signatureSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_DigestInit(uint session, ref CK_MECHANISM.NativeUnix mech);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_DigestUpdate(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_DigestKey(uint session, uint key);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_DigestFinal(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);
  [UnmanagedFunctionPointer(CallingConvention.Cdecl)] internal delegate uint type_C_Digest(uint session,
    [MarshalAs(UnmanagedType.LPArray)] byte[] src, int srcSize, [MarshalAs(UnmanagedType.LPArray)] byte[] dst, ref long dstSize);


  [StructLayout(LayoutKind.Sequential)] internal class CK_FUNCTION_LIST
  {
    public CK_VERSION.NativeUnix version;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_Initialize C_Initialize;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_Finalize C_Finalize;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GetInfo C_GetInfo;
    IntPtr C_GetFunctionList;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GetSlotList C_GetSlotList;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GetSlotInfo C_GetSlotInfo;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GetTokenInfo C_GetTokenInfo;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GetMechanismList C_GetMechanismList;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GetMechanismInfo C_GetMechanismInfo;
    IntPtr C_InitToken;
    IntPtr C_InitPIN;
    IntPtr C_SetPIN;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_OpenSession C_OpenSession;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_CloseSession C_CloseSession;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_CloseAllSessions C_CloseAllSessions;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GetSessionInfo C_GetSessionInfo;
    IntPtr C_GetOperationState;
    IntPtr C_SetOperationState;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_Login C_Login;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_Logout C_Logout;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_CreateObject C_CreateObject;
    IntPtr C_CopyObject;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_DestroyObject C_DestroyObject;
    IntPtr C_GetObjectSize;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GetAttributeValue C_GetAttributeValue;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_SetAttributeValue C_SetAttributeValue;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_FindObjectsInit C_FindObjectsInit;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_FindObjects C_FindObjects;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_FindObjectsFinal C_FindObjectsFinal;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_EncryptInit C_EncryptInit;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_Encrypt C_Encrypt;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_EncryptUpdate C_EncryptUpdate;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_EncryptFinal C_EncryptFinal;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_DecryptInit C_DecryptInit;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_Decrypt C_Decrypt;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_DecryptUpdate C_DecryptUpdate;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_DecryptFinal C_DecryptFinal;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_DigestInit C_DigestInit;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_Digest C_Digest;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_DigestUpdate C_DigestUpdate;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_DigestKey C_DigestKey;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_DigestFinal C_DigestFinal;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_SignInit C_SignInit;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_Sign C_Sign;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_SignUpdate C_SignUpdate;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_SignFinal C_SignFinal;
    IntPtr C_SignRecoverInit;
    IntPtr C_SignRecover;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_VerifyInit C_VerifyInit;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_Verify C_Verify;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_VerifyUpdate C_VerifyUpdate;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_VerifyFinal C_VerifyFinal;
    IntPtr C_VerifyRecoverInit;
    IntPtr C_VerifyRecover;
    IntPtr C_DigestEncryptUpdate;
    IntPtr C_DecryptDigestUpdate;
    IntPtr C_SignEncryptUpdate;
    IntPtr C_DecryptVerifyUpdate;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GenerateKey C_GenerateKey;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GenerateKeyPair C_GenerateKeyPair;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_WrapKey C_WrapKey;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_UnwrapKey C_UnwrapKey;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_DeriveKey C_DeriveKey;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_SeedRandom C_SeedRandom;
    [MarshalAs(UnmanagedType.FunctionPtr)]      internal type_C_GenerateRandom C_GenerateRandom;
    IntPtr C_GetFunctionStatus;
    IntPtr C_CancelFunction;
    IntPtr C_WaitForSlotEvent;
  }

  internal static CK_FUNCTION_LIST fl = new CK_FUNCTION_LIST();

  static LibraryUnix()
  {
    IntPtr module = dlopen("libekmpkcs11.so", RTLD_NOW);
    if (module == IntPtr.Zero) throw new CKR_Exception(CK.CKR_FUNCTION_FAILED, "libekmpkcs11.so is not loaded");
    IntPtr ptr_C_GetFunctionList = dlsym(module, "C_GetFunctionList");

    if (ptr_C_GetFunctionList == IntPtr.Zero) throw new CKR_Exception(CK.CKR_FUNCTION_FAILED);

    type_C_GetFunctionList C_GetFunctionList = (type_C_GetFunctionList)Marshal.GetDelegateForFunctionPointer(ptr_C_GetFunctionList, typeof(type_C_GetFunctionList));

    IntPtr funcListPtr;
    CKR_Exception.check(C_GetFunctionList(out funcListPtr));

    Marshal.PtrToStructure(funcListPtr, fl);
  }

}

}
