using EFModel;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using WebApi.Controllers;

namespace WebApi
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
      services.AddDbContext<FirmaContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Firma")));
      services.AddTransient<MjestoController>();

      services.AddControllers()
              .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
              .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc(Constants.ApiVersion, new OpenApiInfo
        {
          Title = "RPPP Web API",
          Version = Constants.ApiVersion
        });
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        //e.g. include comment from EFModel (not needed, bust just for demonstration)
        xmlFile = $"{typeof(EFModel.Artikl).Assembly.GetName().Name}.xml";
        xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      //middleware order https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0#middleware-order-1
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.RoutePrefix = "docs";
        c.DocumentTitle = "RPPP Web Api";
        c.SwaggerEndpoint($"../swagger/{Constants.ApiVersion}/swagger.json", "RPPP WebAPI");
      });

      app.UseStaticFiles();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
