using Contract.DTOs;
using Contract.Queries;
using DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class DrzaveLookupQueryHandler : IRequestHandler<DrzaveLookupQuery, IEnumerable<TextValue<string>>>
  {    
    private readonly FirmaContext ctx;


    public DrzaveLookupQueryHandler(FirmaContext ctx)
    {
      this.ctx = ctx;
    } 

    public async Task<IEnumerable<TextValue<string>>> Handle(DrzaveLookupQuery query, CancellationToken cancellationToken)
    {
      var list = await ctx.Drzava
                          .OrderBy(a => a.NazDrzave)
                          .Select(a => new TextValue<string>
                          {
                            Value = a.OznDrzave,
                            Text = a.NazDrzave
                          })
                          .ToListAsync(cancellationToken);
      return list;
    }   
  }
}
