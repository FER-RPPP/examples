namespace MVC.Models
{
  public partial class Stavka
  {
    public decimal CijenaStavke
    {
      get
      {
        return KolArtikla * JedCijArtikla * (1 - PostoRabat);
      }
    }
  }
}
