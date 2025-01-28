using MediatR;

namespace Contract.Commands
{
  public class AddCity : IRequest<int>
  {
    public string CityName { get; set; }

    public int PostalCode { get; set; }

    public string CountryCode { get; set; }
    public string PostalName { get; set; }
  }
}
