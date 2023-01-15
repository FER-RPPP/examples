using ConsoleClient;

_ = await CityClient.AddCity(7, "A city with invalid postal code", "HR");
int? id = await CityClient.AddCity(1234, "A demo city with valid postal code", "HR");
if (id.HasValue)
{
  await CityClient.UpdateCity(id.Value, 1235, "new name for the city", "BA");
  await CityClient.GetCity(id.Value);
  Console.WriteLine("ENTER to continue");
  Console.ReadLine();
  await CityClient.DeleteCity(id.Value);
}
