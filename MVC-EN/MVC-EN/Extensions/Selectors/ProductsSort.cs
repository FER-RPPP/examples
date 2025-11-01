using MVC_EN.Models;
using System;
using System.Linq;

namespace MVC_EN.Extensions.Selectors;

public static class ProductsSort
{
  public static IQueryable<Product> ApplySort(this IQueryable<Product> query, int sort, bool ascending)
  {
    System.Linq.Expressions.Expression<Func<Product, object?>>? orderSelector = null;
    switch (sort)
    {
      case 1:
        orderSelector = a => a.PhotoChecksum; //makes sense if we would like to get all products with the photo at the beginning
        break;
      case 2:
        orderSelector = a => a.ProductNumber;
        break;
      case 3:
        orderSelector = a => a.ProductName;
        break;
      case 4:
        orderSelector = a => a.UnitName;
        break;
      case 5:
        orderSelector = a => a.Price;
        break;
      case 6:
        orderSelector = a => a.IsService;
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
