using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MVC_EN;
using MVC_EN.Controllers;
using MVC_EN.Models;
using MVC_EN.ViewModels;

namespace MVC.UnitTests.Controllers.Countries
{
  public class Index
  {
    private readonly IOptionsSnapshot<AppSettings> options;
    private readonly ITempDataDictionary tempData;
    private const int PageSize = 7;

    public Index()
    {
      var tempDataMock = new Mock<ITempDataDictionary>();
      tempData = tempDataMock.Object;


      var mockOptions = new Mock<IOptionsSnapshot<AppSettings>>();
      var appSettings = new AppSettings
      {
        PageSize = PageSize
      };
      mockOptions.SetupGet(options => options.Value)
                 .Returns(appSettings);
      options = mockOptions.Object;
    }

    [Fact]
    [Trait("Category", "CountriesController")]
    public void RedirectsToCreate_WhenNoCountries()
    {
      // Arrange      
      var mockLogger = new Mock<ILogger<CountriesController>>();

      var dbOptions = new DbContextOptionsBuilder<FirmContext>()
                          .UseInMemoryDatabase(databaseName: nameof(RedirectsToCreate_WhenNoCountries))
                          .Options;

      using var context = new FirmContext(dbOptions);
      var controller = new CountriesController(context, options, mockLogger.Object);
      controller.TempData = tempData;

      // Act
      var result = controller.Index();

      // Assert
      var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
      Assert.Equal("Create", redirectToActionResult.ActionName);

      mockLogger.Verify(l => l.Log(LogLevel.Information,
                                  It.IsAny<EventId>(),
                                  It.IsAny<object>(),
                                  It.IsAny<Exception>(),
                                  (Func<object, Exception?, string>)It.IsAny<object>())
                               , Times.Once());
    }

    [Trait("Category", "CountriesController")]
    [Theory]    
    [InlineData(1)]
    [InlineData(6)]
    public async Task Returns_CorrectPageData(int page)
    {
      int totalNumber = 40;
      // Arrange      
      var mockLogger = new Mock<ILogger<CountriesController>>();

      var dbOptions = new DbContextOptionsBuilder<FirmContext>()
                          .UseInMemoryDatabase(databaseName: nameof(Returns_CorrectPageData) + page)
                          .Options;
      using var context = new FirmContext(dbOptions);
      await AddCountries(context, totalNumber);

      var controller = new CountriesController(context, options, mockLogger.Object);
      controller.TempData = tempData;

      // Act
      var result = controller.Index(page:page, sort:1, ascending : false);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      CountriesViewModel model = Assert.IsType<CountriesViewModel>(viewResult.Model);
      viewResult.Model.Should().BeOfType<CountriesViewModel>(); //same as above, but using FluentAssertions

      model.PagingInfo.TotalItems.Should().Be(totalNumber);
      model.PagingInfo.ItemsPerPage.Should().Be(PageSize);

      int expectedItemsCount = Math.Min(PageSize, totalNumber - (page - 1) * PageSize);
      model.Countries.Count().Should().Be(expectedItemsCount);

      for(int i=0; i<expectedItemsCount; i++)
      {
        int expectedId = totalNumber - i - 1 - (page - 1) * PageSize;        
        Assert.Equal(model.Countries.ElementAt(i).CountryCode, $"{expectedId:D2}" );
      }
    }

    private async Task AddCountries(FirmContext context, int count)
    {
      for (int i = 0; i < count; i++)
      {
        string id = $"{i:D2}";
        context.Add(new Country
        {
          CountryCode = id,
          CountryName = id
        });
      }
      await context.SaveChangesAsync();
    }
  }
}
