using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using MVC.Controllers;
using MVC.Models;
using MVC.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MVC.UnitTests.Controllers.AutoComplete
{
  //NOTE: Uses real data!
  public class Mjesto
  {    
    private readonly IOptionsSnapshot<AppSettings> options;
    private readonly DbContextOptionsBuilder<FirmaContext> dbContextBuilder;

    public Mjesto()
    {
      //Arrange
      var builder = new ConfigurationBuilder()
                          .AddUserSecrets("Firma");                          
      var configuration = builder.Build();
      dbContextBuilder = new DbContextOptionsBuilder<FirmaContext>()
                            .UseSqlServer(configuration.GetConnectionString("Firma"));

      var mockOptions = new Mock<IOptionsSnapshot<AppSettings>>();
      var appSettings = new AppSettings
      {
        AutoCompleteCount = 10
      };
      mockOptions.SetupGet(options => options.Value)
                 .Returns(appSettings);
      options = mockOptions.Object;
    }

    [Trait("Category", "AutoCompleteController")]
    [Theory]
    [InlineData("varaždin", "Varaždin", "Split")]
    [InlineData("VARAŽDIN", "Varaždin", "Split")]
    [InlineData("araž", "Varaždin", "Split")]
    [InlineData("ždin", "Varaždin", "Split")]
    public async Task ReturnsCity_UsingCaseInsensitiveSubstring(string term, string expectedCity, string unexpectedCity)
    {      
      //Act
      using var ctx = new FirmaContext(dbContextBuilder.Options);
      var controller = new AutoCompleteController(ctx, options);
      IEnumerable<IdLabel> result = await controller.Mjesto(term);

      //Assert      
      result.Any(l => l.Label.EndsWith(expectedCity)).Should().BeTrue();
      result.Any(l => l.Label.EndsWith(unexpectedCity)).Should().BeFalse();
    }

    [Trait("Category", "AutoCompleteController")]
    [Theory]
    [InlineData("a", 20)]    
    [InlineData("a", 50)]
    [InlineData("a", 0)]
    public async Task Returns_SubsetOfAllCities(string term, int count)
    {
      //Arrange
      var mockOptions = new Mock<IOptionsSnapshot<AppSettings>>();
      var appSettings = new AppSettings
      {
        AutoCompleteCount = count
      };
      mockOptions.SetupGet(options => options.Value)
                 .Returns(appSettings);

      //Act
      using var ctx = new FirmaContext(dbContextBuilder.Options);
      var controller = new AutoCompleteController(ctx, mockOptions.Object);
      IEnumerable<IdLabel> result = await controller.Mjesto(term);

      //Assert      
      result.Count().Should().Be(count);      
    }
  }
}
