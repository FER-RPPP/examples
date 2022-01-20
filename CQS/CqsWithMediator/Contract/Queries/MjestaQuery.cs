using MediatR;
using System.Collections.Generic;

namespace Contract.Queries
{
  public class MjestaQuery : IRequest<IEnumerable<DTOs.Mjesto>>
  {
    public string SearchText { get; set; }
    public int? From { get; set; }
    public int? Count { get; set; }
    public SortInfo Sort { get; set; }
  }
}
