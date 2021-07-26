namespace unbound.cryptoki {

public struct CK_OBJECT_HANDLE
{
  public uint Handle { get; }
  internal CK_OBJECT_HANDLE(uint handle)
  {
    Handle = handle;
  }
}

} //namespace unbound.cryptoki