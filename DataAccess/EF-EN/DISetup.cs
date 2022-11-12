using EF_EN.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EF_EN
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
                              .AddDbContext<FirmContext>(options => {
                                options.UseSqlServer(configuration.GetConnectionString("Firm"));
                              }, contextLifetime: ServiceLifetime.Transient)
                             .BuildServiceProvider();
      return provider;
    }
  }
}
