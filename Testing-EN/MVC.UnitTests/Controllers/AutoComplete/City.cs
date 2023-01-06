using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using MVC_EN;
using MVC_EN.Controllers;
using MVC_EN.Models;
using MVC_EN.ViewModels;

namespace MVC.UnitTests.Controllers.AutoComplete
{
  //NOTE: Uses real data!
  public class City
  {    
    private readonly IOptionsSnapshot<AppSettings> options;
    private readonly DbContextOptionsBuilder<FirmContext> dbContextBuilder;

    public City()
    {
      //Arrange
      var builder = new ConfigurationBuilder().AddUserSecrets("Firm");                          
      var configuration = builder.Build();
      dbContextBuilder = new DbContextOptionsBuilder<FirmContext>()
                            .UseSqlServer(configuration.GetConnectionString("Firm"));

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
      using var ctx = new FirmContext(dbContextBuilder.Options);
      var controller = new AutoCompleteController(ctx, options);
      IEnumerable<IdLabel> result = await controller.Cities(term);

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
      using var ctx = new FirmContext(dbContextBuilder.Options);
      var controller = new AutoCompleteController(ctx, mockOptions.Object);
      IEnumerable<IdLabel> result = await controller.Cities(term);

      //Assert      
      result.Count().Should().Be(count);      
    }
  }
}
