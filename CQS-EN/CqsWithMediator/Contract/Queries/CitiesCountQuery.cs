using MediatR;

namespace Contract.Queries
{
  public class CitiesCountQuery : IRequest<int>
  {
    public string SearchText { get; set; }
  }
}
