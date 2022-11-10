using MVC.Models;
using System.Linq.Expressions;

namespace MVC.Extensions.Selectors
{
  public static class DrzavaSort
  {
    public static IQueryable<Drzava> ApplySort(this IQueryable<Drzava> query, int sort, bool ascending)
    {
      Expression<Func<Drzava, object>> orderSelector = sort switch
      {
        1 => d => d.OznDrzave,
        2 => d => d.NazDrzave,
        3 => d => d.Iso3drzave,
        4 => d => d.SifDrzave,
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
}
