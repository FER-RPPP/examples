using EFModel;
using GraphQL.Server.Ui.Voyager;
using GraphQLServer.SetupGraphQL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region services
builder.Services.AddDbContext<FirmaContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Firma")));
builder.Services
        .AddGraphQLServer()
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

#region configure middleware pipeline
//middleware order https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/#middleware-order
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.MapGraphQL();

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