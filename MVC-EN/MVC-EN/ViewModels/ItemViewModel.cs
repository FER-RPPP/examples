using System.ComponentModel.DataAnnotations;

namespace MVC_EN.ViewModels;

public class ItemViewModel
{
  public int ItemId { get; set; }
  public int ProductNumber { get; set; }
  public string ProductName { get; set; }
  [Range(1e-6, double.MaxValue, ErrorMessage = "Quantity must be positive number")]
  public decimal Quantity { get; set; }
  public decimal UnitPrice { get; set; }
  [Range(0, 1, ErrorMessage = "Discount should be in rage [0,1]")]
  public decimal Discount { get; set; }

  public decimal ItemPrice
  {
    get
    {
      return Quantity * UnitPrice * (1 - Discount);
    }
  }
}
