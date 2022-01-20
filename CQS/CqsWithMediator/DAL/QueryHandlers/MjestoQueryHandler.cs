using Contract.Queries;
using DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class MjestoQueryHandler : IRequestHandler<MjestoQuery, Contract.DTOs.Mjesto>
  {
    private readonly FirmaContext ctx;

    public MjestoQueryHandler(FirmaContext ctx)
    {
      this.ctx = ctx;
    }

    public async Task<Contract.DTOs.Mjesto> Handle(MjestoQuery query, CancellationToken cancellationToken)
    {
      var mjesto = await ctx.Mjesto
                            .Where(m => m.IdMjesta == query.Id)
                            .Select(m => new Contract.DTOs.Mjesto
                            {
                              IdMjesta = m.IdMjesta,
                              NazivDrzave = m.OznDrzaveNavigation.NazDrzave,
                              NazivMjesta = m.NazMjesta,
                              OznDrzave = m.OznDrzave,
                              PostBrojMjesta = m.PostBrMjesta,
                              PostNazivMjesta = m.PostNazMjesta
                            })
                            .FirstOrDefaultAsync(cancellationToken);
      return mjesto;
    }
  }
}
