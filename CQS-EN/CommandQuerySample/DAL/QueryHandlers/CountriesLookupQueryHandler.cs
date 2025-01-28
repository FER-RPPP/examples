using Contract.DTOs;
using Contract.Queries;
using Contract.QueryHandlers;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.QueryHandlers
{
  public class CountriesLookupQueryHandler : ICountriesLookupQueryHandler
  {    
    private readonly FirmContext ctx;


    public CountriesLookupQueryHandler(FirmContext ctx)
    {
      this.ctx = ctx;
    } 

    public async Task<IEnumerable<TextValue<string>>> Handle(CountriesLookupQuery query)
    {
      var list = await ctx.Countries
                          .OrderBy(a => a.CountryName)
                          .Select(a => new TextValue<string>
                          {
                            Value = a.CountryCode,
                            Text = a.CountryName
                          })
                          .ToListAsync();
      return list;
    }
  }
}
