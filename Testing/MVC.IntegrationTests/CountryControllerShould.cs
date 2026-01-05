using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace MVC.IntegrationTests;

public class CountryControllerShould : IClassFixture<CustomWebApplicationFactory<CountryControllerShould>>
{
  private readonly CustomWebApplicationFactory<CountryControllerShould> factory;

  public CountryControllerShould(CustomWebApplicationFactory<CountryControllerShould> factory)
  {
    this.factory = factory;
  }

  [Fact]
  [Trait("Category", "Integration Tests")]    
  public async Task Redirect_When_DbContainsNoCountries()
  {      
    string url = "/Drzava/Index";
    var client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      AllowAutoRedirect = false
    });
    var response = await client.GetAsync(url);
    response.StatusCode.Should().Be(HttpStatusCode.Redirect);
    response.Headers.Location
      .Should().NotBeNull()
      .And.Be("/Drzava/Create");
  }   
}
