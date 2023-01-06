using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using MVC_EN.ViewModels;
using System.Net;
using System.Text.Json;

namespace MVC.IntegrationTests
{
  public class AutoCompleteShould : IClassFixture<WebApplicationFactory<Program>>
  {
    private readonly WebApplicationFactory<Program> factory;

    public AutoCompleteShould(WebApplicationFactory<Program> factory)
    {
      this.factory = factory;     
    }
    
    [Trait("Category", "Integration Tests")]
    [Theory]
    [InlineData("velika gor", 10410, 10380)]   
    [InlineData("gre", 10020, 10000)]
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
          .HaveCountGreaterThanOrEqualTo(postcodes.Length)
          .And.Contain(l => l.Label.StartsWith(postcode.ToString())
                          && l.Label.Contains(input, StringComparison.OrdinalIgnoreCase),
                because: "it should contain post code " + postcode
                          );
      }            
    }
  }
}
