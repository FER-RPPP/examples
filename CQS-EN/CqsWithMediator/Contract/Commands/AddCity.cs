using MediatR;

namespace Contract.Commands;

public class AddCity : IRequest<int>
{
  public required string CityName { get; set; }

  public int PostalCode { get; set; }

  public required string CountryCode { get; set; }
  public string? PostalName { get; set; }
}
