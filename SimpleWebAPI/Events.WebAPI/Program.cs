using Events.WebAPI;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
var logger = LogManager.Setup().GetCurrentClassLogger();

try
{
  logger.Debug("init main");
  builder.Host.UseNLog(new NLogAspNetCoreOptions() { 
    RemoveLoggerFactoryFilter = false 
  });

  var app = builder
                .ConfigureServices()  // dependency injection configuration
                .ConfigurePipeline(); // middleware pipeline configuration
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