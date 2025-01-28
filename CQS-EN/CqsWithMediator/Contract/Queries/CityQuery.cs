using Contract.DTOs;
using MediatR;

namespace Contract.Queries
{
  public class CityQuery : IRequest<City>
  {
    public int Id { get; set; }
  }
}
