namespace OpenAPIClients
{
  public partial class MjestoViewModel
  {
    public override string ToString()
    {
      return $"{IdMjesta}. {PostBrojMjesta} {NazivMjesta} {PostNazivMjesta} ({NazivDrzave})";
    }
  }
}
