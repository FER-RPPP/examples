using Using;

try
{
  C a1 = new C("A1");

  using (C b2 = new C("B2"))
  using (C d4 = new C("D4"))
  {
    C c3 = new C("C3");
    throw new Exception("It is time for an exception");
  }
  a1.Dispose();
}
catch (Exception exc)
{
  Console.WriteLine("Exc: " + exc.Message);
  //throw exc;
  //throw;
}