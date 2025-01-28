using CommandQueryCore;
using Contract.Queries;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class SearchCitiesQueryHandler : IQueryHandler<SearchCitiesQuery, IEnumerable<Contract.DTOs.City>>
  {
    private readonly FirmContext ctx;

    public SearchCitiesQueryHandler(FirmContext ctx)
    {
      this.ctx = ctx;
    }
    public async Task<IEnumerable<Contract.DTOs.City>> Handle(SearchCitiesQuery query)
    {
      var dbQuery = ctx.Cities.AsQueryable();

      if (!string.IsNullOrWhiteSpace(query.CityName))
      {
        dbQuery = dbQuery.Where(m => m.CityName == query.CityName);
      }
      if (!string.IsNullOrWhiteSpace(query.CountryCode))
      {
        dbQuery = dbQuery.Where(m => m.CountryCode == query.CountryCode);
      }
      if (query.PostalCode.HasValue)
      {
        dbQuery = dbQuery.Where(m => m.PostalCode == query.PostalCode.Value);
      }
      
      var list = await dbQuery.Select(m => new Contract.DTOs.City
                              {
                                CityId = m.CityId,
                                CountryName = m.CountryCodeNavigation.CountryName,
                                CityName = m.CityName,
                                CountryCode = m.CountryCode,
                                PostalCode = m.PostalCode,
                                PostalName = m.PostalName
                              })
                              .ToListAsync();
      return list;
    }
  }
}
