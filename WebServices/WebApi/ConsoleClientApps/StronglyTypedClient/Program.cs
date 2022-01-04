using OpenAPIClients;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StronglyTypedClient
{
  class Program
  {
    const string url = "https://localhost:44377";
    static async Task Main(string[] args)
    {
      using var httpClient = new HttpClient();
      var apiClient = new RPPPClient(url, httpClient);

      var model = new MjestoViewModel
      {
        NazivMjesta = "Neki probni naziv mjesta",
        OznDrzave = "HR",
        PostBrojMjesta = 12345,
        PostNazivMjesta = "Poštanski naziv mjesta"
      };

      await apiClient.DodajMjestoAsync(model);

      var mjesta = await apiClient.DohvatiMjestaAsync(0, 10, null, model.NazivMjesta);
      if (mjesta.Count == 1)
      {
        var mjesto = mjesta.First();
        Console.WriteLine(mjesto);

        ++mjesto.PostBrojMjesta;
        mjesto.OznDrzave = "BA";
        mjesto.NazivMjesta = "Neki drugi naziv";

        await apiClient.AzurirajMjestoAsync(mjesto.IdMjesta, mjesto);


        mjesto = await apiClient.DohvatiMjestoAsync(mjesto.IdMjesta);
        Console.WriteLine(mjesto);

        await apiClient.ObrisiMjestoAsync(mjesto.IdMjesta);
      }

    }
  }
}
