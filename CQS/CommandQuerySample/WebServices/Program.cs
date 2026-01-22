using System.Reflection;
using CommandQueryCore;
using Contract.CommandHandlers;
using Contract.Commands;
using Contract.Queries;
using Contract.QueryHandlers;
using Contract.Validation.CommandValidators;
using DAL.CommandHandlers;
using DAL.Models;
using DAL.QueryHandlers;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using WebServices;
using WebServices.Controllers;
using WebServices.Util;

var builder = WebApplication.CreateBuilder(args);

#region Configure services
builder.Services.AddDbContext<FirmaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firma")));
builder.Services
       .AddControllers()
       .AddJsonOptions(configure => configure.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services
       .AddValidatorsFromAssemblyContaining<AddMjestoValidator>();

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

builder.Services.AddAutoMapper(cfg => { }, typeof(WebServices.Util.ApiModelsMappingProfile));
#region Register handlers
builder.Services.AddTransient<IMjestaQueryHandler, MjestaQueryHandler>();
builder.Services.AddTransient<IMjestoQueryHandler, MjestoQueryHandler>();
builder.Services.AddTransient<IMjestoCountQueryHandler, MjestoCountQueryHandler>();

builder.Services.AddTransient<IQueryHandler<SearchMjestoQuery, List<Contract.DTOs.Mjesto>>, SearchMjestoQueryHandler>();

builder.Services.AddTransient<ICommandHandler<DeleteMjesto>, MjestoCommandHandler>();

builder.Services.AddTransient<MjestoCommandHandler>();
builder.Services.AddTransient<ICommandHandler<AddMjesto, int>, ValidateCommandBeforeHandle<AddMjesto, int, MjestoCommandHandler>>();
builder.Services.AddTransient<ICommandHandler<UpdateMjesto>, ValidateCommandBeforeHandle<UpdateMjesto, MjestoCommandHandler>>();

builder.Services.AddTransient<IDrzaveLookupQueryHandler, DrzaveLookupQueryHandler>();
#endregion
#endregion

var app = builder.Build();

#region Configure middleware pipeline.
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#middleware-order-1

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