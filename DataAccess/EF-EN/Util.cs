namespace EF_EN;

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

  public static int ReadNumber()
  {
    int number;
    while (!int.TryParse(Console.ReadLine(), out number)) ;
    return number;
  }
}