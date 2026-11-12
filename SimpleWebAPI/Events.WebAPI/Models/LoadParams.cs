using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Events.WebAPI.Models;

/// <summary>
/// Map lazy loading parameters (e.g. from PrimeNG table)
/// </summary>   
public class LoadParams 
{
  /// <summary>
  /// Page to load
  /// </summary>
  public int Page { get; set; } = 1;
  /// <summary>
  /// Number of elements to return
  /// </summary>
  public int? PageSize { get; set; }
  /// <summary>
  /// Name of a column. Must be same as in corresponding DTO object, case insensitive
  /// In case of multiple columns, separated them with comma and without spaces
  /// </summary>
  public string? Sort { get; set; }
  /// <summary>
  /// 1 ascending, -1 descending
  /// </summary>
  public int? SortOrder { get; set; }

  /// <summary>
  /// Sieve style filter string
  /// </summary>
  public string? Filters { get; set; }

  [BindNever] public bool Ascending => SortOrder == null || SortOrder != -1;
  [BindNever] public bool Descending => !Ascending;
  internal string GetSort(string defaultSort)
  {
    // defaultSort is used when Sort is null and PageSize is not null,
    // i.e. when we want to load a page of data but no sort is specified
    string? sort = Sort ?? (PageSize.HasValue ? defaultSort : null);
    if (sort == null)
    {
      return string.Empty;
    }
    else if (SortOrder == null || SortOrder != -1)
    {
      return sort;
    }
    else
    {
      return "-" + sort;
    } 
  }
}
