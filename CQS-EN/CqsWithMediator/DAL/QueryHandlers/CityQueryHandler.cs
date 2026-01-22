using Contract.Queries;
using DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.QueryHandlers;

public class CityQueryHandler : IRequestHandler<CityQuery, Contract.DTOs.City?>
{
  private readonly FirmContext ctx;

  public CityQueryHandler(FirmContext ctx)
  {
    this.ctx = ctx;
  }

  public async Task<Contract.DTOs.City?> Handle(CityQuery query, CancellationToken cancellationToken)
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
                        .FirstOrDefaultAsync(cancellationToken);
    return city;
  }
}
