namespace ConsoleClient;

class CityClient
{
  const string url = "https://localhost:7295/cities";

  public static async Task UpdateCity(int id, int postalCode, string cityName, string countryCode)
  {
    var city = new City(id, postalCode, cityName, cityName, countryCode, string.Empty);
    using var client = new HttpClient();
    var response = await client.PutAsJsonAsync($"{url}/{id}", city);
    Console.WriteLine("Update result: {0} {1}", (int)response.StatusCode, response.ReasonPhrase);
  }

  public static async Task GetCity(int id)
  {
    using var client = new HttpClient();
    var response = await client.GetAsync($"{url}/{id}");
    if (response.IsSuccessStatusCode)
    {
      var city = await response.Content.ReadAsAsync<City>();
      Console.Write($"City with id {id} is as follows: ");
      Console.WriteLine(city.ToString());
    }
    else
    {
      Console.WriteLine($"Unsuccessful  get for city id {id}");
    }
  }

  public async static Task DeleteCity(int id)
  {
    using var client = new HttpClient();
    var response = await client.DeleteAsync($"{url}/{id}");
    Console.WriteLine("Delete result: {0} {1}", (int)response.StatusCode, response.ReasonPhrase);
  }

  public static async Task<int?> AddCity(int postalCode, string cityName, string countryCode)
  {
    var city = new City(default, postalCode, cityName, cityName, countryCode, string.Empty);

    using var client = new HttpClient();
    var response = await client.PostAsJsonAsync<City>(url, city);
    if (response.IsSuccessStatusCode)
    {
      Console.WriteLine($"City has been added and available at {response.Headers.Location}");
      city = await response.Content.ReadAsAsync<City>();
      Console.WriteLine(city);
      return city.CityId;
    }
    else
    {
      string content = await response.Content.ReadAsStringAsync();
      Console.WriteLine("City cannot be added: {0} {1} \n {2}", (int)response.StatusCode, response.ReasonPhrase, content);
      return null;
    }
  }
}
