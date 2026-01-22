using System.ComponentModel.DataAnnotations;

namespace Contract.DTOs;

public class Mjesto
{
  public int IdMjesta { get; set; }
  /// <summary>
  /// City postcode / Poštanski broj
  /// </summary>
  [Range(10, 90000)] public int PostBrojMjesta { get; set; }
  /// <summary>
  /// City name/naziv mjesta
  /// </summary>
  [Required] public required string NazivMjesta { get; set; }
  public string? PostNazivMjesta { get; set; }
  [Required] public required string OznDrzave { get; set; }
  public required string NazivDrzave { get; set; } = string.Empty;
}
