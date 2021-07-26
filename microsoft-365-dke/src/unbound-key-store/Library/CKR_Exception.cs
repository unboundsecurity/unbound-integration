using System;

namespace unbound.cryptoki {

public class CKR_Exception : System.Exception
{
  public uint Error;
  public string Function;
  internal CKR_Exception(uint error)
  {
    Error = error;
  }
  internal CKR_Exception(uint error, String function)
  {
    Error = error;
    Function = function;
  }
  internal static void check(uint rv)
  {
    if (rv != 0) throw new CKR_Exception(rv);
  }
  internal static void check(uint rv, String function)
  {
    if (rv != 0) throw new CKR_Exception(rv, function);
  }
}
  
} // namespace unbound.cryptoki