using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace MVC.IntegrationTests
{
  public class CountriesControllerShould : IClassFixture<CustomWebApplicationFactory<CountriesControllerShould>>
  {
    private readonly CustomWebApplicationFactory<CountriesControllerShould> factory;

    public CountriesControllerShould(CustomWebApplicationFactory<CountriesControllerShould> factory)
    {
      this.factory = factory;
    }

    [Fact]
    [Trait("Category", "Integration Tests")]    
    public async Task Redirect_When_DbContainsNoCountries()
    {      
      string url = "/Countries/Index";
      var client = factory.CreateClient(new WebApplicationFactoryClientOptions
      {
        AllowAutoRedirect = false
      });
      var response = await client.GetAsync(url);
      response.StatusCode.Should().Be(HttpStatusCode.Redirect);
      response.Headers.Location
        .Should().NotBeNull()
        .And.Be("/Countries/Create");
    }   
  }
}
