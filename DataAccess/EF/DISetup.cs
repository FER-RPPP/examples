using EF.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EF;

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
                            .AddDbContext<FirmaContext>(options => {
                              options.UseSqlServer(configuration.GetConnectionString("Firma"));
                              //options.LogTo(Console.Write, minimumLevel: LogLevel.Information); //too verbose -> solved using logging configuration
                            }, contextLifetime: ServiceLifetime.Transient)
                           .BuildServiceProvider();
    return provider;
  }
}
