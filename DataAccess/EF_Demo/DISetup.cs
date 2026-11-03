using EF_Demo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#if POSTGRES
using Events.DAL.Data.Postgres;
#else
using Events.DAL.Data.MSSQL;
#endif

namespace EF_Demo;

internal static class DISetup
{
  public static IHost BuildHost(string[] args)
  {
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

#if POSTGRES
    builder.Services.AddDbContextFactory<EventsContext>(options => {
      options.UseNpgsql(builder.Configuration.GetConnectionString("EventsPostgres"));
    });
#else
    builder.Services.AddDbContextFactory<EventsContext>(options =>
    {
      options.UseSqlServer(builder.Configuration.GetConnectionString("EventsMssql"));
    });
#endif
   
    builder.Services.AddTransient<Demo>();

    return builder.Build();
  }
}