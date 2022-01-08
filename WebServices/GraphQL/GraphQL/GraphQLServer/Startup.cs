using EFModel;
using GraphQL.Server.Ui.Voyager;
using GraphQLServer.SetupGraphQL;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GraphQLServer
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    
    public void ConfigureServices(IServiceCollection services)
    {
      //Note: Cannot run in parallel, see https://chillicream.com/docs/hotchocolate/integrations/entity-framework for workaround
      services.AddDbContext<FirmaContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Firma")));
      services.AddGraphQLServer()
              .SetPagingOptions(new PagingOptions
              {                
                DefaultPageSize = 20,
                MaxPageSize = 1000,
                IncludeTotalCount = true                
              })
              .AddProjections()
              .AddFiltering()
              .AddSorting()
              .AddQueryType<Queries>()
              .AddMutationType<Mutations>();                     
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      //middleware order https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-5.0#middleware-order-1
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseStaticFiles();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGraphQL();
      });

      app.UseGraphQLVoyager(new VoyagerOptions()
                                {
                                  GraphQLEndPoint = "/graphql"
                                },
                             "/graphql-voyager");
    }
  }
}
