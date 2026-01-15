using EFModel;
using GraphQL.Server.Ui.Voyager;
using GraphQLServer.SetupGraphQL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Configure services

// add/configure services

builder.Services.AddDbContext<FirmContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firm")));
builder.Services.AddGraphQLServer()
                .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = builder.Environment.IsDevelopment())
                .ModifyPagingOptions(options =>
                {
                  options.DefaultPageSize = 20;
                  options.MaxPageSize = 1000;
                  options.IncludeTotalCount = true;
                })
                .AddProjections()
                .AddFiltering()
                .AddSorting()
                .AddQueryType<Queries>()
                .AddMutationType<Mutations>();
#endregion

var app = builder.Build();

#region Configure middleware pipeline.
//// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-8.0#middleware-order-1

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.MapGraphQL(); // automatically adds Nitro UI at /graphql, see https://chillicream.com/docs/nitro/integrations/hot-chocolate for details

app.UseGraphQLVoyager("/voyager", new VoyagerOptions() { GraphQLEndPoint = "graphql" });
app.UseGraphQLGraphiQL(
    "/",                               // url to host Playground at
    new GraphQL.Server.Ui.GraphiQL.GraphiQLOptions
    {
      GraphQLEndPoint = "/graphql",         // url of GraphQL endpoint
      SubscriptionsEndPoint = "/graphql",   // url of GraphQL endpoint
    });
#endregion

app.Run();