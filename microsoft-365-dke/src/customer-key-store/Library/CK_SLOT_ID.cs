namespace unbound.cryptoki {

public struct CK_SLOT_ID
{
  public uint Id { get; }

  internal CK_SLOT_ID(uint id)
  {
    Id = id;
  }
}

} //namespace unbound.cryptoki