namespace MVC_EN.Models;

public partial class Item
{
  public decimal ItemPrice
  {
    get
    {
      return Quantity * UnitPrice * (1 - Discount);
    }
  }
}