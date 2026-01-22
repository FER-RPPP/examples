using Contract.Queries;
using Contract.QueryHandlers;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.QueryHandlers;

public class CityQueryHandler : ICityQueryHandler
{
  private readonly FirmContext ctx;

  public CityQueryHandler(FirmContext ctx)
  {
    this.ctx = ctx;
  }

  public async Task<Contract.DTOs.City?> Handle(CityQuery query)
  {
    var city = await ctx.Cities
                        .Where(m => m.CityId == query.Id)
                        .Select(m => new Contract.DTOs.City
                        {
                          CityId = m.CityId,
                          CountryName = m.CountryCodeNavigation.CountryName,
                          CityName = m.CityName,
                          CountryCode = m.CountryCode,
                          PostalCode = m.PostalCode,
                          PostalName = m.PostalName
                        })
                        .FirstOrDefaultAsync();
    return city;
  }
}
