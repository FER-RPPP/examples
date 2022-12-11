namespace MVC_EN.Models;

public class ViewPartner
{
  public int PartnerId { get; set; }
  public string PartnerType { get; set; }
  public string VatNumber { get; set; }
  public string PartnerName { get; set; }
  public string PartnerTypeText => PartnerType == "P" ? "Person" : "Company";
}
