using CommandQueryCore;

namespace Contract.Queries
{
  public class CitiesCountQuery : IQuery<int>
  {
    public string SearchText { get; set; }
  }
}
