using MediatR;

namespace Contract.Commands
{
  public class UpdateCity : IRequest
  {
    public int CityId { get; set; }
    public string CityName { get; set; }

    public int PostalCode { get; set; }

    public string CountryCode { get; set; }
    public string PostalName { get; set; }
  }
}
