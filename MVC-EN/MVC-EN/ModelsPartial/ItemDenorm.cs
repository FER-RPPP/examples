using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_EN.Models;

public class ItemDenorm
{
  public string VatNumber { get; set; }
  public string PartnerName { get; set; }
  public int DocumentId { get; set; }
  public DateTime DocumentDate { get; set; }
  public decimal DocumentAmount { get; set; }
  public int ItemId { get; set; }
  public int ProductNumber { get; set; }
  public decimal Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal Discount { get; set; }
  public string ProductName { get; set; }
  [NotMapped]
  public string DocumentUrl { get; set; }
}
