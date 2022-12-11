namespace MVC_EN.ViewModels;
public class CitiesViewModel
{
  public IEnumerable<CityViewModel> Cities { get; set; }
  public PagingInfo PagingInfo { get; set; }
}
