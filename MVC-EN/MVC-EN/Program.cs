using Microsoft.Extensions.Configuration;
using MVC_EN.Models;
using MVC_EN;
using NLog;
using NLog.Web;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MVC_EN.ModelsValidation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using QuestPDF.Infrastructure;
using OfficeOpenXml;

var logger = NLog.LogManager.Setup().GetCurrentClassLogger();
logger.Debug("init main");
try
{
  var builder = WebApplication.CreateBuilder(args);
  builder.Host.UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = false });

  #region Configure services
  var appSection = builder.Configuration.GetSection("AppSettings");
  builder.Services.Configure<AppSettings>(appSection);

  builder.Services.AddDbContext<FirmContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firm")));

  builder.Services.AddControllersWithViews();

  builder.Services
          .AddFluentValidationAutoValidation()
          .AddFluentValidationClientsideAdapters()
          .AddValidatorsFromAssemblyContaining<CountryValidator>();
  #endregion

  var app = builder.Build();

  #region configure middleware pipeline
  //middleware order https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/#middleware-order

  if (app.Environment.IsDevelopment())
  {
    app.UseDeveloperExceptionPage();
  }

  app.UseStaticFiles();

  app.UseRouting();

  app.MapControllerRoute("Cities and products",
          "{action}/{controller:regex(^(Cities|Products)$)}/Page{page}/Sort{sort:int}/ASC-{ascending:bool}/{id?}",
          new { action = "Index" }
          );

  app.MapDefaultControllerRoute();

  #endregion

  QuestPDF.Settings.License = LicenseType.Community;
  ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
  app.Run();
}
catch (Exception exception)
{
  // NLog: catch setup errors
  logger.Error(exception, "Stopped program because of exception");
  throw;
}
finally
{
  // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
  NLog.LogManager.Shutdown();
}

public partial class Program { }