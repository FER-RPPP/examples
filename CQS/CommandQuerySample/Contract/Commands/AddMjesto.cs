namespace Contract.Commands;

public class AddMjesto
{
  public required string NazivMjesta { get; set; }

  public int PostBrojMjesta { get; set; }

  public required string OznDrzave { get; set; }
  public string? PostNazivMjesta { get; set; }
}
