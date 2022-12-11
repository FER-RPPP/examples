using MVC_EN.Models;

namespace MVC_EN.ViewModels
{
  public class CountriesViewModel
  {
    public IEnumerable<Country> Countries { get; set; }
    public PagingInfo PagingInfo { get; set; }
  }
}
