using EFModel;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using ODataApi;

var builder = WebApplication.CreateBuilder(args);

#region Configure services

// add/configure services

builder.Services.AddDbContext<FirmaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firma")));
builder.Services.AddControllers()
              .AddOData(opt => opt.AddRouteComponents("odata", FirmaODataModelBuilder.GetEdmModel()));
#endregion

var app = builder.Build();

#region Configure middleware pipeline.
// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0#middleware-order-1

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

#endregion

app.Run();