using DSAOpenApiClients;

const string url = "https://localhost:7295";

using var httpClient = new HttpClient();
var apiClient = new FirmWebApiClient(url, httpClient);

var model = new CityViewModel
{
  CityName = "Some demo name",
  CountryCode = "HR",
  PostalCode = 12345,
  PostalName = "Some postal name"
};

await apiClient.AddCityAsync(model);

var cities = await apiClient.GetCitiesAsync(0, 10, null, model.CityName);
if (cities.Count == 1)
{
  var city = cities.First();
  Console.WriteLine(city);

  ++city.PostalCode;
  city.CountryCode = "BA";
  city.CityName = "Some other name";

  await apiClient.UpdateCityAsync(city.CityId, city);


  city = await apiClient.GetCityByIdAsync(city.CityId);
  Console.WriteLine(city);

  await apiClient.DeleteCityAsync(city.CityId);
}
