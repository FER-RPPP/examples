using Contract.DTOs;
using Contract.Queries;
using DAL.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.QueryHandlers;

public class CountriesLookupQueryHandler : IRequestHandler<CountriesLookupQuery, IEnumerable<TextValue<string>>>
{    
  private readonly FirmContext ctx;


  public CountriesLookupQueryHandler(FirmContext ctx)
  {
    this.ctx = ctx;
  } 

  public async Task<IEnumerable<TextValue<string>>> Handle(CountriesLookupQuery query, CancellationToken cancellationToken)
  {
    var list = await ctx.Countries
                        .OrderBy(a => a.CountryName)
                        .Select(a => new TextValue<string>
                        {
                          Value = a.CountryCode,
                          Text = a.CountryName
                        })
                        .ToListAsync(cancellationToken);
    return list;
  }   
}
