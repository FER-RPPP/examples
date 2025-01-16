using EFModel;
using Microsoft.EntityFrameworkCore;

namespace GraphQLServer.SetupGraphQL
{
  public class Queries
  {
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<City> GetCities([Service] FirmContext ctx) => ctx.Cities.AsNoTracking();

    
    [UseFiltering]
    [UseSorting]
    public IQueryable<City> GetCitiesForCountry([Service] FirmContext ctx, string country)
      => ctx.Cities.AsNoTracking()
            .Where(m => m.CountryCode == country);

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Country> GetCountries([Service] FirmContext ctx) => ctx.Countries.AsNoTracking();
   
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Document> GetDocuments([Service] FirmContext ctx) => ctx.Documents.AsNoTracking();

  }
}
