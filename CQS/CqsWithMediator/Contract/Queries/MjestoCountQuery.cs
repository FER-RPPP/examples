using MediatR;

namespace Contract.Queries
{
  public class MjestoCountQuery : IRequest<int>
  {
    public string SearchText { get; set; }
  }
}
