using EFModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using System.Reflection;
using WebApi;
using WebApi.Controllers;
using WebApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region Configure services

// add/configure services
builder.Services
       .AddControllers()
       .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddDbContext<FirmContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firm")));
builder.Services.AddTransient<CitiesController>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc(Constants.ApiVersion, new OpenApiInfo
  {
    Title = "Firm Web API",
    Version = Constants.ApiVersion
  });
  var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
  var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  c.IncludeXmlComments(xmlPath);

  //e.g. include comment from EFModel (not needed, bust just for demonstration)
  xmlFile = $"{typeof(EFModel.Product).Assembly.GetName().Name}.xml";
  xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
  c.IncludeXmlComments(xmlPath);
});
#endregion

var app = builder.Build();

#region Configure middleware pipeline.
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0#middleware-order-1

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.RoutePrefix = "docs";
  c.DocumentTitle = "Firm Web Api";
  c.SwaggerEndpoint($"../swagger/{Constants.ApiVersion}/swagger.json", "Firm WebAPI");
});


app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
#endregion

app.Run();
