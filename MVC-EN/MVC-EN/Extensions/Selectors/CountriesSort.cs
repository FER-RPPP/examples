using MVC_EN.Models;
using System.Linq.Expressions;

namespace MVC_EN.Extensions.Selectors;

public static class CountriesSort
{
  public static IQueryable<Country> ApplySort(this IQueryable<Country> query, int sort, bool ascending)
  {
    Expression<Func<Country, object?>>? orderSelector = sort switch
    {
      1 => d => d.CountryCode,
      2 => d => d.CountryName,
      3 => d => d.CountryIso3,
      _ => null
    };
    
    if (orderSelector != null)
    {
      query = ascending ?
             query.OrderBy(orderSelector) :
             query.OrderByDescending(orderSelector);
    }

    return query;
  }
}
