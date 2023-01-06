using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MVC_EN.Models;

namespace MVC.IntegrationTests
{
  public class CustomWebApplicationFactory<TTest> : WebApplicationFactory<Program> 
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.ConfigureServices(services =>
      {
        #region Replace database context with an in-memory context
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType ==
                typeof(DbContextOptions<FirmContext>));
        if (descriptor != null)
        {
          services.Remove(descriptor);
        }

        services.AddDbContext<FirmContext>(options =>
        {
          options.UseInMemoryDatabase("Firm-" + typeof(TTest).Name); //database per test collection          
        });

        var sp = services.BuildServiceProvider();

        using (var scope = sp.CreateScope())
        {
          var scopedServices = scope.ServiceProvider;
          var db = scopedServices.GetRequiredService<FirmContext>();          
          db.Database.EnsureCreated();    
          //TO DO: Fill db with common data
        }
        #endregion        
      });
    }    
  }
}
