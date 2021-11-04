using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVC.Models;

namespace MVC
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
      var appSection = Configuration.GetSection("AppSettings");
      services.Configure<AppSettings>(appSection);

      services.AddDbContext<FirmaContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Firma")));

      services.AddControllersWithViews()
              .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());              
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
        endpoints.MapControllerRoute("Places and articles",
              "{action}/{controller:regex(^(Mjesto|Artikl)$)}/Page{page}/Sort{sort:int}/ASC-{ascending:bool}/{id?}",
              new { action = "Index"}
              );

        endpoints.MapDefaultControllerRoute();               
      });

      
    }
  }
}
