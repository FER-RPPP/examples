using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleClient
{
  class Program
  {
    const string url = "https://localhost:44377/mjesto";
    static async Task Main(string[] args)
    {
      _ = await DodajMjesto(7, "Neko mjesto s lošim poštanskim brojem", "HR");
      int? id = await DodajMjesto(1234, "Neko mjesto s valjanim poštanskim brojem", "HR");
      if (id.HasValue)
      {
        await AzurirajMjesto(id.Value, 1235, "Novi naziv", "BA");
        await DohvatiMjesto(id.Value);
        Console.WriteLine("ENTER za nastavak");
        Console.ReadLine();
        await ObrisiMjesto(id.Value);
      }

    }


    private static async Task AzurirajMjesto(int id, int pbr, string naziv, string oznDrzave)
    {
      var mjesto = new Mjesto
      {
        IdMjesta = id,
        PostBrojMjesta = pbr,
        NazivMjesta = naziv,
        PostNazivMjesta = naziv,
        OznDrzave = oznDrzave
      };

      using (var client = new HttpClient())
      {
        var response = await client.PutAsJsonAsync($"{url}/{id}", mjesto);
        Console.WriteLine("Rezultat ažuriranja: {0} {1}", (int)response.StatusCode, response.ReasonPhrase);
      }
    }

    private static async Task DohvatiMjesto(int id)
    {
      using (var client = new HttpClient())
      {
        var response = await client.GetAsync($"{url}/{id}");
        if (response.IsSuccessStatusCode)
        {
          var mjesto = await response.Content.ReadAsAsync<Mjesto>();
          Console.Write($"Mjesto s id-om {id} je : ");
          Console.WriteLine(mjesto.ToString());
        }
        else
        {
          Console.WriteLine($"Neuspješan dohvat mjesta {id}");
        }
      }
    }

    private async static Task ObrisiMjesto(int id)
    {
      using (var client = new HttpClient())
      {
        var response = await client.DeleteAsync($"{url}/{id}");
        Console.WriteLine("Rezultat brisanja: {0} {1}", (int)response.StatusCode, response.ReasonPhrase);
      }
    }

    private static async Task<int?> DodajMjesto(int pbr, string naziv, string oznDrzave)
    {
      var mjesto = new Mjesto
      {
        PostBrojMjesta = pbr,
        NazivMjesta = naziv,
        PostNazivMjesta = naziv,
        OznDrzave = oznDrzave
      };

      using (var client = new HttpClient())
      {
        var response = await client.PostAsJsonAsync<Mjesto>(url, mjesto);
        if (response.IsSuccessStatusCode)
        {
          Console.WriteLine($"Mjesto dodano i može se dohvatiti na adresi {response.Headers.Location}");          
          mjesto = await response.Content.ReadAsAsync<Mjesto>();          
          Console.WriteLine(mjesto);
          return mjesto.IdMjesta;
        }        
        else 
        {
          string content = await response.Content.ReadAsStringAsync();
          Console.WriteLine("Dodavanje neuspješno: {0} {1} \n {2}", (int)response.StatusCode, response.ReasonPhrase, content);
          return null;
        }
      }
    }
  }
}
