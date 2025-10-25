using System.ComponentModel.DataAnnotations;

namespace MVC.ViewModels;

public class MjestoViewModel
{
  public int IdMjesta { get; set; }
  [Display(Name = "Poštanski broj mjesta"), Required, Range(1000,99999)] public int PostBrojMjesta { get; set; }
  [Display(Name = "Naziv mjesta"), Required] public required string NazivMjesta { get; set; }
  [Display(Name = "Poštanski naziv mjesta")] public string? PostNazivMjesta { get; set; }
  [Display(Name = "Država"), Required] public required string OznDrzave { get; set; }
  public required string NazivDrzave { get; set; } = string.Empty;
}
