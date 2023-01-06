#nullable disable
namespace ReflectionBenchmark
{
  public record PersonDest
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string Address { get; set; }
    public DateTime Birthday { get; set; }
  }
}
