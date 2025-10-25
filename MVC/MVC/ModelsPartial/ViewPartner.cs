namespace MVC.Models;

public class ViewPartner
{
  public int IdPartnera { get; set; }
  public required string TipPartnera { get; set; }
  public required string OIB { get; set; }
  public required string Naziv { get; set; }
  public string TipPartneraText => TipPartnera == "O" ? "Osoba" : "Tvrtka";
}
