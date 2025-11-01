using MVC_EN.Models;
using System.Linq.Expressions;

namespace MVC_EN.Extensions.Selectors;

public static class CitiesSort
{
  public static IQueryable<City> ApplySort(this IQueryable<City> query, int sort, bool ascending)
  {
    Expression<Func<City, object?>>? orderSelector = null;
    switch (sort)
    {
      case 1:
        orderSelector = m => m.PostalCode;
        break;
      case 2:
        orderSelector = m => m.CityName;
        break;
      case 3:
        orderSelector = m => m.PostalName;
        break;
      case 4:
        orderSelector = m => m.CountryCodeNavigation.CountryName;
        break;
    }
    if (orderSelector != null)
    {
      query = ascending ?
             query.OrderBy(orderSelector) :
             query.OrderByDescending(orderSelector);
    }

    return query;
  }
}
