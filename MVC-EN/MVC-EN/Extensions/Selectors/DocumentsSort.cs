using MVC_EN.Models;

namespace MVC_EN.Extensions.Selectors;

public static class DocumentsSort
{
  public static IQueryable<ViewDocument> ApplySort(this IQueryable<ViewDocument> query, int sort, bool ascending)
  {
    System.Linq.Expressions.Expression<Func<ViewDocument, object>>? orderSelector = null;
    switch (sort)
    {
      case 1:
        orderSelector = d => d.DocumentId;
        break;
      case 2:
        orderSelector = d => d.PartnerName;
        break;
      case 3:
        orderSelector = d => d.DocumentDate;
        break;
      case 4:
        orderSelector = d => d.Amount;
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
