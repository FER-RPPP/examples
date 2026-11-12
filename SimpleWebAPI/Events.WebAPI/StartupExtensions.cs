using Events.EF.Data;
using Events.WebAPI.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Sieve.Models;
using Sieve.Services;
using System.Reflection;
using System.Xml.Linq;

namespace Events.WebAPI;

public static class StartupExtensions
{
  const string apiVersion = "0.0.1";
  public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
  {
    builder.Services
           .AddControllers()
           .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

    builder.Services.AddDbContext<EventsContext>(options =>
       options.UseNpgsql(builder.Configuration.GetConnectionString("EventsPostgres")));

    builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));
    builder.Services.AddScoped<ISieveProcessor, SieveProcessor>();

    builder.Services.AddValidatorsFromAssemblyContaining(typeof(RegistrationDTOValidator));

    #region Swagger settings
    builder.Services.AddSwaggerGen(c =>
    {
      c.SwaggerDoc(apiVersion, new OpenApiInfo
      {
        Title = "RPPP/DoSA Simple Web API",
        Version = apiVersion
      });
      Assembly[] assemblies = { 
        Assembly.GetExecutingAssembly(), 
        typeof(Events.EF.Data.EventsContext).Assembly 
      };
      foreach (var assembly in assemblies)
      {
        var xmlFile = $"{assembly.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
          c.IncludeXmlComments(xmlPath);
      }
    });
    #endregion

    return builder.Build();
  }

  public static WebApplication ConfigurePipeline(this WebApplication app)
  {
    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
      c.RoutePrefix = "docs";
      c.DocumentTitle = "RPPP/DoSA Simple WebAPI";
      c.SwaggerEndpoint($"../swagger/{apiVersion}/swagger.json", "RPPP/DoSA Simple WebAPI");
    });

    app.UseRouting();

    //set CORS if you want to allow cross-origin requests (e.g. from a different port or domain, from a SPA application, etc.)
    //app.UseCors(builder =>
    //{
    //  builder
    //   .AllowAnyOrigin()
    //   .AllowAnyMethod()
    //   .AllowAnyHeader();
    //});

    app.MapControllers();

    return app;
  }
}