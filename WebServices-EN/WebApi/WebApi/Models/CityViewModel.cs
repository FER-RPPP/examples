using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class CityViewModel
{
  public int CityId { get; set; }
  [Range(10, 90000)] public int PostalCode { get; set; }
  [Required] public required string CityName { get; set; }
  public string? PostalName { get; set; }
  [Required] public required string CountryCode { get; set; }
  public string CountryName { get; set; } = string.Empty;
}
