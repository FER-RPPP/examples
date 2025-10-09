//ValueTuples https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct

(int a, int b, int _) t1 = CreateRandomTriple();
Console.WriteLine(t1);
Console.WriteLine(t1.b);

var (_, b, _) = CreateRandomTriple();
Console.WriteLine(b);


var t2 = CreateRandomTriple();
Console.WriteLine(t2);
Console.WriteLine(t2.Item2);

(int, int, int) CreateRandomTriple()
{
  Random r = new Random();
  var triple = (r.Next(100), r.Next(100), r.Next(100));
  Console.WriteLine($"Created {triple}");
  return triple;
}
 