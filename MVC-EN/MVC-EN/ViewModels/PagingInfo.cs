namespace MVC_EN.ViewModels;

public class PagingInfo
{
  public int TotalItems { get; set; }
  public int ItemsPerPage { get; set; }
  public int CurrentPage { get; set; }
  public bool Ascending { get; set; }
  public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
  public int Sort { get; set; }
}
