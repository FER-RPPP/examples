using MediatR;
using System.Collections.Generic;

namespace Contract.Queries;

public class CitiesQuery : IRequest<List<DTOs.City>>
{
  public string? SearchText { get; set; }
  public int? From { get; set; }
  public int? Count { get; set; }
  public SortInfo? Sort { get; set; }
}
