using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MVC.IntegrationTests
{
  public class HomeControllerShould : IClassFixture<WebApplicationFactory<Startup>>
  {
    private readonly WebApplicationFactory<Startup> factory;

    public HomeControllerShould(WebApplicationFactory<Startup> factory) 
    {
      this.factory = factory;
    }
    
    [Theory]
    [Trait("Category", "Integration Tests")]
    [InlineData("Home/Index")]
    [InlineData("")]
    public async Task ServeHomePage(string url)
    {      
      var client = factory.CreateClient();
      var response = await client.GetAsync(url);
      response.StatusCode.Should().Be(HttpStatusCode.OK);
      string content = await response.Content.ReadAsStringAsync();
      content.Should().Contain("RPPP");
    }
  }
}
