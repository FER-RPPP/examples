using EFModel;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace GraphQLServer.SetupGraphQL
{
  public class Queries
  {
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Mjesto> GetCities([Service] FirmaContext ctx) => ctx.Mjesto.AsNoTracking();

    
    [UseFiltering]
    [UseSorting]
    public IQueryable<Mjesto> GetCitiesForCountry([Service] FirmaContext ctx, string country)
      => ctx.Mjesto.AsNoTracking()
            .Where(m => m.OznDrzave == country);

    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Drzava> GetCountries([Service] FirmaContext ctx) => ctx.Drzava.AsNoTracking();
   
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Dokument> GetDocuments([Service] FirmaContext ctx) => ctx.Dokument.AsNoTracking();

  }
}
