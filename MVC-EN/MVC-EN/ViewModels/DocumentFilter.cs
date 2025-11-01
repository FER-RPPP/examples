using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MVC_EN.Models;

namespace MVC_EN.ViewModels;

public class DocumentFilter : IPageFilter
{
  public int? PartnerId { get; set; }
  public string PartnerName { get; set; } = string.Empty;
  [DataType(DataType.Date)]   
  public DateTime? DateFrom { get; set; }
  [DataType(DataType.Date)]   
  public DateTime? DateTo { get; set; }
  public decimal? AmountFrom { get; set; }
  public decimal? AmountTo { get; set; }

  public bool IsEmpty()
  {
    bool active = PartnerId.HasValue
                  || DateFrom.HasValue
                  || DateTo.HasValue
                  || AmountFrom.HasValue
                  || AmountTo.HasValue;
    return !active;
  }

  public override string ToString()
  {
    return string.Format("{0}-{1}-{2}-{3}-{4}",
        PartnerId,
        DateFrom?.ToString("dd.MM.yyyy"),
        DateTo?.ToString("dd.MM.yyyy"),
        AmountFrom,
        AmountTo);
  }

  public static DocumentFilter FromString(string s)
  {
    var filter = new DocumentFilter();
    if (!string.IsNullOrEmpty(s))
    {
      string[] arr = s.Split('-', StringSplitOptions.None);

      if (arr.Length == 5)
      {
        filter.PartnerId = string.IsNullOrWhiteSpace(arr[0]) ? new int?() : int.Parse(arr[0]);
        filter.DateFrom = string.IsNullOrWhiteSpace(arr[1]) ? new DateTime?() : DateTime.ParseExact(arr[1], "dd.MM.yyyy", CultureInfo.InvariantCulture);
        filter.DateTo = string.IsNullOrWhiteSpace(arr[2]) ? new DateTime?() : DateTime.ParseExact(arr[2], "dd.MM.yyyy", CultureInfo.InvariantCulture);
        filter.AmountFrom = string.IsNullOrWhiteSpace(arr[3]) ? new decimal?() : decimal.Parse(arr[3]);
        filter.AmountTo = string.IsNullOrWhiteSpace(arr[4]) ? new decimal?() : decimal.Parse(arr[4]);
      }
    }
    
    return filter;
  }

  public IQueryable<ViewDocument> Apply(IQueryable<ViewDocument> query)
  {
    if (PartnerId.HasValue)
    {
      query = query.Where(d => d.PartnerId == PartnerId.Value);
    }
    if (DateFrom.HasValue)
    {
      query = query.Where(d => d.DocumentDate >= DateFrom.Value);
    }
    if (DateTo.HasValue)
    {
      query = query.Where(d => d.DocumentDate <= DateTo.Value);
    }
    if (AmountFrom.HasValue)
    {
      query = query.Where(d => d.Amount >= AmountFrom.Value);
    }
    if (AmountTo.HasValue)
    {
      query = query.Where(d => d.Amount <= AmountTo.Value);
    }
    return query;
  }   
}
