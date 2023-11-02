namespace EF;

public static class Util
{
  public static void EnterToContinue(bool clearConsole = true) 
  {
    Console.WriteLine("ENTER to continue"); 
    Console.ReadLine(); 
    if (clearConsole)
    {
      Console.Clear();
    }    
  }
}
