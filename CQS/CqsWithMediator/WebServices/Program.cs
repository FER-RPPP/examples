using Contract.Validation.DTOs;
using DAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using WebServices;
using WebServices.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using System;
using FluentValidation;
using Contract.Validation;

var builder = WebApplication.CreateBuilder(args);

#region Configure services
builder.Services.AddDbContext<FirmaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firma")));
builder.Services.AddControllers()
        .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services
       .AddFluentValidationAutoValidation()
       .AddValidatorsFromAssemblyContaining<MjestoValidator>();

builder.Services.AddSwaggerGen(c =>
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

builder.Services.AddTransient<MjestoController>();

builder.Services.AddAutoMapper(typeof(WebServices.Util.ApiModelsMappingProfile));

builder.Services.AddMediatR(cfg => {
  cfg.RegisterServicesFromAssembly(typeof(DAL.CommandHandlers.MjestoCommandHandler).Assembly);
  cfg.AddOpenBehavior(typeof(ValidationPipeline<,>));
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
  c.DocumentTitle = "RPPP Web Api";
  c.SwaggerEndpoint($"../swagger/{Constants.ApiVersion}/swagger.json", "RPPP WebAPI");
});


app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
#endregion

app.Run();