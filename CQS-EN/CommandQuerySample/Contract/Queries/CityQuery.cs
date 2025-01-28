using CommandQueryCore;
using Contract.DTOs;

namespace Contract.Queries
{
  public class CityQuery : IQuery<City>
  {
    public int Id { get; set; }
  }
}
