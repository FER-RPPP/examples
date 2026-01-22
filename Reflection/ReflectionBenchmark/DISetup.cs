
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReflectionBenchmark.Model;

namespace ReflectionBenchmark
{
  internal class DISetup
  {
    public static ServiceProvider BuildDI()
    {
      var configuration = new ConfigurationBuilder()
                              .AddJsonFile("appsettings.json")
                              .AddUserSecrets<Program>() //foldername is defined in .csproj of the project
                              .Build();

      IServiceCollection services = new ServiceCollection();

      var provider = services.AddLogging(configure => {
                                configure.AddConfiguration(configuration.GetSection("Logging"));
                                configure.AddConsole();
                              })
                              .AddSingleton<IConfiguration>(configuration)
                              .AddDbContext<FirmContext>(options => {
                                options.UseSqlServer(configuration.GetConnectionString("Firm"));
                              }, contextLifetime: ServiceLifetime.Transient)
                             .BuildServiceProvider();
      return provider;
    }
  }
}
