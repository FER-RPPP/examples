using MVC_EN.Models;

namespace MVC_EN.ViewModels;

public class PartnersViewModel
{
  public IEnumerable<ViewPartner> Partners { get; set; }
  public PagingInfo PagingInfo { get; set; }
}
