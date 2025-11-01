namespace MVC_EN.Models;

public class ViewPartner
{
  public int PartnerId { get; set; }
  public required string PartnerType { get; set; }
  public required string VatNumber { get; set; }
  public required string PartnerName { get; set; }
  public string PartnerTypeText => PartnerType == "P" ? "Person" : "Company";
}
