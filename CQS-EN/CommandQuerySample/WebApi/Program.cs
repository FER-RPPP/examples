using CommandQueryCore;
using Contract.CommandHandlers;
using Contract.Commands;
using Contract.Queries;
using Contract.QueryHandlers;
using DAL.CommandHandlers;
using DAL.Models;
using DAL.QueryHandlers;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WebApi;
using WebApi.Controllers;
using WebApi.Util;

var builder = WebApplication.CreateBuilder(args);

#region Configure services

// add/configure services
builder.Services
       .AddControllers()
       .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services
       .AddFluentValidationAutoValidation()       
       .AddValidatorsFromAssemblyContaining<Contract.DTOs.City>();

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
});

builder.Services.AddAutoMapper(typeof(ApiModelsMappingProfile));

#region Register handlers
builder.Services.AddTransient<ICitiesQueryHandler, CitiesQueryHandler>();
builder.Services.AddTransient<ICityQueryHandler, CityQueryHandler>();
builder.Services.AddTransient<ICitiesCountQueryHandler, CitiesCountQueryHandler>();

builder.Services.AddTransient<IQueryHandler<SearchCitiesQuery, IEnumerable<Contract.DTOs.City>>, SearchCitiesQueryHandler>();

builder.Services.AddTransient<ICommandHandler<DeleteCity>, CityCommandHandlers>();

builder.Services.AddTransient<CityCommandHandlers>();
builder.Services.AddTransient<ICommandHandler<AddCity, int>, ValidateCommandBeforeHandle<AddCity, int, CityCommandHandlers>>();
builder.Services.AddTransient<ICommandHandler<UpdateCity>, ValidateCommandBeforeHandle<UpdateCity, CityCommandHandlers>>();

builder.Services.AddTransient<ICountriesLookupQueryHandler, CountriesLookupQueryHandler>();
#endregion

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
