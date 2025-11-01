namespace MVC_EN.ViewModels;

public class ProductViewModel
{
  public int ProductNumber { get; set; }
  public required string ProductName { get; set; }
  public required string UnitName { get; set; }
  public decimal Price { get; set; }
  public bool IsService { get; set; }
  public string? Description { get; set; }
  public bool HasPhoto { get; set; }
  public int? ImageHash { get; set; }
}
