using MVC_EN.Models;

namespace MVC_EN.Extensions.Selectors;

public static class PartnersSort
{
  public static IQueryable<ViewPartner> ApplySort(this IQueryable<ViewPartner> query, int sort, bool ascending)
  {
    System.Linq.Expressions.Expression<Func<ViewPartner, object>>? orderSelector = null;
    switch (sort)
    {
      case 1:
        orderSelector = p => p.PartnerId;
        break;
      case 2:
        orderSelector = p => p.PartnerType;
        break;
      case 3:
        orderSelector = p => p.VatNumber;
        break;
      case 4:
        orderSelector = p => p.PartnerName;
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
