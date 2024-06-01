using EFModel;
using GraphQL.Server.Ui.Voyager;
using GraphQLServer.SetupGraphQL;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region services
builder.Services.AddDbContext<FirmaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firma")));
builder.Services
        .AddGraphQLServer()
        .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = builder.Environment.IsDevelopment())
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

app.UseStaticFiles();

app.UseRouting();


app.MapGraphQL();

app.UseGraphQLVoyager("/voyager", new VoyagerOptions() { GraphQLEndPoint = "graphql" });
app.UseGraphQLPlayground(
    "/",                               // url to host Playground at
    new GraphQL.Server.Ui.Playground.PlaygroundOptions
    {
      GraphQLEndPoint = "/graphql",         // url of GraphQL endpoint
      SubscriptionsEndPoint = "/graphql",   // url of GraphQL endpoint
    });

#endregion

app.Run();