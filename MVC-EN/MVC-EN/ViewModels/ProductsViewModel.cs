namespace MVC_EN.ViewModels;

public class ProductsViewModel
{
  public IEnumerable<ProductViewModel> Products { get; set; }
  public PagingInfo PagingInfo { get; set; }
}
