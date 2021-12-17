using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MVC.Models;
using MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

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
        var ctx = scope.ServiceProvider.GetRequiredService<FirmaContext>();
        if (ctx.Drzava.Count() == 0)
        {
          Drzava d = new Drzava
          {
            OznDrzave = "HR",
            NazDrzave = "Croatia"
          };
          d.Mjesto.Add(new Mjesto
          {
            NazMjesta = "Zagreb",
            PostBrMjesta = 10000
          });
          d.Mjesto.Add(new Mjesto
          {
            NazMjesta = "Velika Gorica",
            PostBrMjesta = 10410
          });
          d.Mjesto.Add(new Mjesto
          {
            NazMjesta = "Velika Mlaka",
            PostBrMjesta = 10408
          });
          d.Mjesto.Add(new Mjesto
          {
            NazMjesta = "Novi Zagreb",
            PostBrMjesta = 10020
          });

          ctx.Add(d);
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
      string url = "/AutoComplete/Mjesto?term=" + input;
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
