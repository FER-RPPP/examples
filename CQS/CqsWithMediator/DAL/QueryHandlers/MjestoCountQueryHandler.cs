using Contract.Queries;
using DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class MjestoCountQueryHandler : IRequestHandler<MjestoCountQuery, int>
  {
    private readonly FirmaContext ctx;

    public MjestoCountQueryHandler(FirmaContext ctx)
    {
      this.ctx = ctx;
    }

    public async Task<int> Handle(MjestoCountQuery query, CancellationToken cancellationToken)
    {
      var dbQuery = ctx.Mjesto.AsQueryable();
      if (!string.IsNullOrWhiteSpace(query.SearchText))
      {
        dbQuery = dbQuery.Where(m => m.NazMjesta.Contains(query.SearchText));
      }
      int count = await dbQuery.CountAsync(cancellationToken);
      return count;
    }

    
  }
}
