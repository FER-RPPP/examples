using Contract.DTOs;
using MediatR;

namespace Contract.Queries
{
  public class MjestoQuery : IRequest<Mjesto>
  {
    public int Id { get; set; }
  }
}
