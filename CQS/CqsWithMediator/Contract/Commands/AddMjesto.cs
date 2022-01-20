using MediatR;

namespace Contract.Commands
{
  public class AddMjesto : IRequest<int>
  {
    public string NazivMjesta { get; set; }

    public int PostBrojMjesta { get; set; }

    public string OznDrzave { get; set; }
    public string PostNazivMjesta { get; set; }
  }
}
