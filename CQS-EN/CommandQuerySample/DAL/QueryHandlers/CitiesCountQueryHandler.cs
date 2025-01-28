using Contract.Queries;
using Contract.QueryHandlers;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class CitiesCountQueryHandler : ICitiesCountQueryHandler
  {
    private readonly FirmContext ctx;

    public CitiesCountQueryHandler(FirmContext ctx)
    {
      this.ctx = ctx;
    }

    public async Task<int> Handle(CitiesCountQuery query)
    {
      var dbQuery = ctx.Cities.AsQueryable();
      if (!string.IsNullOrWhiteSpace(query.SearchText))
      {
        dbQuery = dbQuery.Where(m => m.CityName.Contains(query.SearchText));
      }
      int count = await dbQuery.CountAsync();
      return count;
    }
  }
}
