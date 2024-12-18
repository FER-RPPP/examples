namespace MVC_EN.ViewModels.PDFs;

public class Order
{
  public string VatNumber { get; set; }
  public string PartnerName { get; set; }
  public int DocumentId { get; set; }
  public DateTime DocumentDate { get; set; }
  public decimal Amount { get; set; }
  public IEnumerable<OrderItem> Items { get; set; }
}

public class OrderItem
{
  public int ItemId { get; set; }
  public int ProductNumber { get; set; }
  public decimal Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  public decimal Discount { get; set; }
  public string ProductName { get; set; }
  public decimal ItemPrice => Quantity * UnitPrice * (1 - Discount);
}
