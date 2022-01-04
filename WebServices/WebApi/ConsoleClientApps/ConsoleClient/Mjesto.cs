﻿namespace ConsoleClient
{
  public class Mjesto
  {
    public int IdMjesta { get; set; }
    public int PostBrojMjesta { get; set; }
    public string NazivMjesta { get; set; }
    public string PostNazivMjesta { get; set; }
    public string OznDrzave { get; set; }
    public string NazivDrzave { get; set; }

    public override string ToString()
    {
      return $"{IdMjesta}. {PostBrojMjesta} {NazivMjesta} {PostNazivMjesta} ({NazivDrzave})";
    }
  }
}
