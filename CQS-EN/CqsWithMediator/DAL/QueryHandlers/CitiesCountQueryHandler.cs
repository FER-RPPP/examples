using Contract.Queries;
using DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.QueryHandlers;

public class CitiesCountQueryHandler : IRequestHandler<CitiesCountQuery, int>
{
  private readonly FirmContext ctx;

  public CitiesCountQueryHandler(FirmContext ctx)
  {
    this.ctx = ctx;
  }

  public async Task<int> Handle(CitiesCountQuery query, CancellationToken cancellationToken)
  {
    var dbQuery = ctx.Cities.AsQueryable();
    if (!string.IsNullOrWhiteSpace(query.SearchText))
    {
      dbQuery = dbQuery.Where(m => m.CityName.Contains(query.SearchText));
    }
    int count = await dbQuery.CountAsync(cancellationToken);
    return count;
  }

  
}
