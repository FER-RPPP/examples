using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.OData;

namespace ODataApi
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {      
      services.AddDbContext<EFModel.FirmaContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Firma")));

      services.AddControllers()
              .AddOData(opt => opt.AddRouteComponents("odata", FirmaODataModelBuilder.GetEdmModel()));      
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      //middleware order https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0#middleware-order-1
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
     
      app.UseStaticFiles();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
