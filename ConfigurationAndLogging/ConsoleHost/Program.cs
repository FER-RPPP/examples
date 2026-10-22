using ConsoleHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// See https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host?tabs=appbuilder#host-builder-settings
// for details what is included and in which order.
// The default settings include configuration from
// appsettings.json, appsettings.{Environment}.json, environment variables,
// command line arguments, and user secrets (in development).
var builder = Host.CreateApplicationBuilder(args);
//builder.Configuration.AddJsonFile("missing_one.json", optional: true, reloadOnChange: true);
builder.Services.AddTransient<Demo>();
var host = builder.Build();

Demo demo = host.Services.GetRequiredService<Demo>();
ILogger? logger = host.Services.GetService<ILogger<Program>>();
logger?.LogInformation($"Demo bound from configuration = {demo}"); 
