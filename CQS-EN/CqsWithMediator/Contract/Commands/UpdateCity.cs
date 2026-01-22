using MediatR;

namespace Contract.Commands;

public class UpdateCity : IRequest
{
  public int CityId { get; set; }
  public required string CityName { get; set; }

  public int PostalCode { get; set; }

  public required string CountryCode { get; set; }
  public string? PostalName { get; set; }
}
