namespace Contract.DTOs
{
  public class Mjesto
  {
    public int IdMjesta { get; set; }
    /// <summary>
    /// City postcode / Poštanski broj
    /// </summary>
    public int PostBrojMjesta { get; set; }
    /// <summary>
    /// City name/naziv mjesta
    /// </summary>
    public string NazivMjesta { get; set; }
    public string PostNazivMjesta { get; set; }
    public string OznDrzave { get; set; }
    public string NazivDrzave { get; set; }
  }
}
