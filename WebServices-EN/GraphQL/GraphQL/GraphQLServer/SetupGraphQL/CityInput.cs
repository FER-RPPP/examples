#nullable enable
namespace GraphQLServer.SetupGraphQL
{
  public record CityInput(
    int PostalCode,
    string CityName,
    string? PostalName,
    string CountryCode); 
}
