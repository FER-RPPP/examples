namespace ConsoleClient;

class MjestoClient
{
  const string url = "https://localhost:7295/cities";

  public static async Task AzurirajMjesto(int id, int pbr, string naziv, string oznDrzave)
  {
    var mjesto = new Mjesto(id, pbr, naziv, naziv, oznDrzave, string.Empty);

    using var client = new HttpClient();
    var response = await client.PutAsJsonAsync($"{url}/{id}", mjesto);
    Console.WriteLine("Rezultat ažuriranja: {0} {1}", (int)response.StatusCode, response.ReasonPhrase);
  }

  public static async Task DohvatiMjesto(int id)
  {
    using var client = new HttpClient();

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

  public async static Task ObrisiMjesto(int id)
  {
    using var client = new HttpClient();

    var response = await client.DeleteAsync($"{url}/{id}");
    Console.WriteLine("Rezultat brisanja: {0} {1}", (int)response.StatusCode, response.ReasonPhrase);
  }

  public static async Task<int?> DodajMjesto(int pbr, string naziv, string oznDrzave)
  {
    var mjesto = new Mjesto(default, pbr, naziv, naziv, oznDrzave, string.Empty);

    using var client = new HttpClient();

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
