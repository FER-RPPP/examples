using Contract.Validation;
using Contract.Validation.DTOs;
using DAL.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
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
using WebServices.Controllers;

namespace WebServices
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
      services.AddControllers()
              .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

      services.AddFluentValidationAutoValidation()
              .AddValidatorsFromAssemblyContaining<MjestoValidator>();

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

        //e.g. include comment from Contract project
        xmlFile = $"{typeof(Contract.DTOs.Mjesto).Assembly.GetName().Name}.xml";
        xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
      });

      services.AddTransient<MjestoController>();

      services.AddAutoMapper(typeof(Startup), typeof(Util.ApiModelsMappingProfile));
                  
      services.AddMediatR(cfg => {
        cfg.RegisterServicesFromAssembly(typeof(DAL.CommandHandlers.MjestoCommandHandler).Assembly);        
        cfg.AddOpenBehavior(typeof(ValidationPipeline<,>));
      });
    }
    

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
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
