namespace unbound.cryptoki {

public struct CK_SESSION_HANDLE
{
  public uint Handle { get; }
  internal CK_SESSION_HANDLE(uint handle)
  {
    Handle = handle;
  }
}

} //namespace unbound.cryptoki