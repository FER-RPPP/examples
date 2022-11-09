using Microsoft.Extensions.Configuration;
using MVC.Models;
using MVC;
using NLog;
using NLog.Web;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var logger = NLog.LogManager.Setup().GetCurrentClassLogger();
logger.Debug("init main");
try
{
  var builder = WebApplication.CreateBuilder(args);  
  builder.Host.UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = false });

  #region Configure services
  var appSection = builder.Configuration.GetSection("AppSettings");
  builder.Services.Configure<AppSettings>(appSection);

  builder.Services.AddDbContext<FirmaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firma")));

  builder.Services.AddControllersWithViews();

  builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
  #endregion

  var app = builder.Build();

  #region configure middleware pipeline
  //middleware order https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0#middleware-order

  if (app.Environment.IsDevelopment())
  {
    app.UseDeveloperExceptionPage();
  }

  app.UseStaticFiles();

  app.UseRouting();

  app.UseEndpoints(endpoints =>
  {
    endpoints.MapControllerRoute("Places and articles",
          "{action}/{controller:regex(^(Mjesto|Artikl)$)}/Page{page}/Sort{sort:int}/ASC-{ascending:bool}/{id?}",
          new { action = "Index" }
          );

    endpoints.MapDefaultControllerRoute();
  });

  #endregion

  
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
  
