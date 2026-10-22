using Microsoft.Extensions.Configuration;

namespace ConsoleHost;

public record Demo
{
  public int Key0 { get; set; }
  public string? Key1 { get; set; }
  public string? Key2 { get; set; }

  public Demo(IConfiguration configuration) => configuration.GetSection("Demo").Bind(this);
}
