namespace MVC.ViewModels.PDFs;

public class Kupnja
{
  public string OIB { get; set; }
  public string NazPartnera { get; set; }
  public int IdDokumenta { get; set; }
  public DateTime DatDokumenta { get; set; }
  public decimal IznosDokumenta { get; set; }
  public IEnumerable<StavkaKupnje> Stavke { get; set; }
}

public class StavkaKupnje
{
  public int IdStavke { get; set; }
  public int SifArtikla { get; set; }
  public decimal KolArtikla { get; set; }
  public decimal JedCijArtikla { get; set; }
  public decimal PostoRabat { get; set; }
  public string NazArtikla { get; set; }
  public decimal CijenaStavke => KolArtikla * JedCijArtikla * (1 - PostoRabat);
}
