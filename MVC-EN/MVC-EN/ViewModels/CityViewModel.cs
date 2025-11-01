using System.ComponentModel.DataAnnotations;

namespace MVC_EN.ViewModels;

public class CityViewModel
{
  public int CityId { get; set; }
  [Display(Name = "Postal code"), Required, Range(1000, 99999)] public int PostalCode { get; set; }
  [Display(Name = "City name"), Required] public required string CityName { get; set; }
  [Display(Name = "Postal name")] public string? PostalName { get; set; }
  [Display(Name = "Country"), Required] public required string CountryCode { get; set; }
  public required string CountryName { get; set; } = string.Empty;
}
