using MediatR;
using System.Collections.Generic;

namespace Contract.Queries
{
  public class SearchMjestoQuery : IRequest<IEnumerable<DTOs.Mjesto>>
  {
    public int? PostanskiBroj { get; set; }
    public string OznDrzave { get; set; }
    public string NazivMjesta { get; set; }
  }
}
