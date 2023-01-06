using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MVC_EN.Models;
using MVC_EN.ViewModels;
using System.Net;
using System.Text.Json;

namespace MVC.IntegrationTests
{
  public class InMemoryAutoCompleteShould : IClassFixture<CustomWebApplicationFactory<InMemoryAutoCompleteShould>>
  {
    private readonly CustomWebApplicationFactory<InMemoryAutoCompleteShould> factory;

    public InMemoryAutoCompleteShould(CustomWebApplicationFactory<InMemoryAutoCompleteShould> factory)
    {
      this.factory = factory;
      AddData();
    }

    private void AddData()
    {
      using (var scope = factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
      {
        var ctx = scope.ServiceProvider.GetRequiredService<FirmContext>();
        if (ctx.Countries.Count() == 0)
        {
          Country country = new ()
          {
            CountryCode = "HR",
            CountryName = "Croatia"
          };
          country.Cities.Add(new City
          {
            CityName = "Zagreb",
            PostalCode = 10000
          });
          country.Cities.Add(new City
          {
            CityName = "Velika Gorica",
            PostalCode = 10410
          });
          country.Cities.Add(new City
          {
            CityName = "Velika Mlaka",
            PostalCode = 10408
          });
          country.Cities.Add(new City
          {
            CityName = "Novi Zagreb",
            PostalCode = 10020
          });

          ctx.Add(country);
          ctx.SaveChanges();
        }
      }
    }

    [Trait("Category", "Integration Tests")]
    [Theory]
    [InlineData("Velika", 10410, 10408)]   //in-memory database is case-sensitive 
    [InlineData("Zagreb", 10020, 10000)]
    public async Task ReturnCities(string input, params int[] postcodes)
    {
      string url = "/AutoComplete/Cities?term=" + input;
      var client = factory.CreateClient();
      var response = await client.GetAsync(url);
      response.StatusCode.Should().Be(HttpStatusCode.OK);
      
      var stream = await response.Content.ReadAsStreamAsync();
      
      var data = await JsonSerializer.DeserializeAsync<IEnumerable<IdLabel>>(stream);
      foreach(var postcode in postcodes)
      {
        data.Should()
          .HaveCount(postcodes.Length)
          .And.Contain(l => l.Label.StartsWith(postcode.ToString())
                          && l.Label.Contains(input));
      }            
    }
  }
}
