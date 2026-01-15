using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class MjestoViewModel
{
  public int IdMjesta { get; set; }
  [Range(10, 90000)] public int PostBrojMjesta { get; set; }
  [Required] public required string NazivMjesta { get; set; }
  public string? PostNazivMjesta { get; set; }
  [Required] public required string OznDrzave { get; set; }
  public string NazivDrzave { get; set; } = string.Empty;
}
