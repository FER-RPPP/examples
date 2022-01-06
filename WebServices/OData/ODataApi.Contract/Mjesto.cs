using System.ComponentModel.DataAnnotations;

namespace ODataApi.Contract
{
  /// <summary>
  /// Data transfer object (DTO) za mjesta. Koristi se kao ulazno/izlazni model za web api (OData) s mjestima
  /// </summary>
  public class Mjesto
  {
    /// <summary>
    /// Interni broj koji služi kao identifikator (primarni ključ za mjesta) - samogenerirajući
    /// </summary>
    [Key]
    public int IdMjesta { get; set; }

    /// <summary>
    /// Poštanski broj mjesta
    /// </summary>
    [Required(ErrorMessage = "Poštanski broj je obvezno polje")]
    [Range(10, 99999, ErrorMessage = "Dozvoljeni raspon: 10-99999")]
    public int PostBrojMjesta { get; set; }

    /// <summary>
    /// Naziv mjesta
    /// </summary>
    [Required(ErrorMessage = "Naziv mjesta je obvezno polje")]
    public string NazivMjesta { get; set; }

    /// <summary>
    /// Poštanski naziv mjesta
    /// </summary>
    public string PostNazivMjesta { get; set; }

    /// <summary>
    /// Naziv države u kojoj se mjesto nalazi
    /// </summary>
    public string NazivDrzave { get; set; }

    /// <summary>
    /// oznaka države u kojoj se mjesto nalazi
    /// </summary>
    [Required(ErrorMessage = "Potrebno je unijeti oznaku države")]    
    public string OznDrzave { get; set; }   
  }
}