﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
  /// <summary>
  /// Map lazy loading parameters
  /// </summary>
  public class LoadParams
  {   
    /// <summary>
    /// Starting row index (i.e. how many rows to skip)
    /// </summary>    
    [FromQuery(Name = "jtStartIndex")]
    [Required]
    [Range(0, int.MaxValue)]
    public int StartIndex { get; set; }
    
    /// <summary>
    /// Number of elements to return
    /// </summary>
    [FromQuery(Name = "jtPageSize")]
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "jtPageSize must be positiive number")]
    public int Rows { get; set; }
   
    
    /// <summary>
    /// Name of a column. Must be same as in corresponding DTO object, case insensitive
    /// </summary>
    [FromQuery(Name = "jtSorting")]
    public string? Sort { get; set; }

    /// <summary>
    /// Global filter parameter
    /// </summary>
    [FromQuery(Name = "filter")]
    public string? Filter { get; set; }

    [BindNever]
    public bool Descending => !string.IsNullOrWhiteSpace(Sort) && Sort.EndsWith("DESC", StringComparison.OrdinalIgnoreCase);

    [BindNever]
    public string? SortColumn
    {
      get
      {
        string? column = null;
        if (!string.IsNullOrWhiteSpace(Sort))
        {
          int ind = Sort.IndexOf(' ');
          if (ind != -1)
          {
            column = Sort.Substring(0, ind);
          }
          else
          {
            column = Sort;
          }
        }
        return column;
      }
    }
  }
}
