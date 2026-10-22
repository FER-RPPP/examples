#define DEMO2

using System.Diagnostics;

Console.WriteLine("Start");

#if (DEMO)
Console.WriteLine("Print something...");
#endif

CheckSomething();

Console.WriteLine("End");

[Conditional("DEMO")]
static void CheckSomething()
{
  Console.WriteLine("Print from CheckSomething method");
}

