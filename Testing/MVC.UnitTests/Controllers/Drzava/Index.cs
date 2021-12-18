using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MVC.Controllers;
using MVC.Models;
using MVC.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MVC.UnitTests.Controllers.Drzava
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
      var mockLogger = new Mock<ILogger<DrzavaController>>();

      var dbOptions = new DbContextOptionsBuilder<FirmaContext>()
                          .UseInMemoryDatabase(databaseName: nameof(RedirectsToCreate_WhenNoCountries))
                          .Options;

      using var context = new FirmaContext(dbOptions);
      var controller = new DrzavaController(context, options, mockLogger.Object);
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
                                  (Func<object, Exception, string>)It.IsAny<object>())
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
      var mockLogger = new Mock<ILogger<DrzavaController>>();

      var dbOptions = new DbContextOptionsBuilder<FirmaContext>()
                          .UseInMemoryDatabase(databaseName: nameof(Returns_CorrectPageData) + page)
                          .Options;
      using var context = new FirmaContext(dbOptions);
      await AddCountries(context, totalNumber);

      var controller = new DrzavaController(context, options, mockLogger.Object);
      controller.TempData = tempData;

      // Act
      var result = controller.Index(page:page, sort:1, ascending : false);

      // Assert
      var viewResult = Assert.IsType<ViewResult>(result);
      DrzaveViewModel model = Assert.IsType<DrzaveViewModel>(viewResult.Model);
      viewResult.Model.Should().BeOfType<DrzaveViewModel>(); //same as above, but using FluentAssertions

      model.PagingInfo.TotalItems.Should().Be(totalNumber);
      model.PagingInfo.ItemsPerPage.Should().Be(PageSize);

      int expectedItemsCount = Math.Min(PageSize, totalNumber - (page - 1) * PageSize);
      model.Drzave.Count().Should().Be(expectedItemsCount);

      for(int i=0; i<expectedItemsCount; i++)
      {
        int expectedId = totalNumber - i - 1 - (page - 1) * PageSize;        
        Assert.Equal(model.Drzave.ElementAt(i).OznDrzave, $"{expectedId:D2}" );
      }
    }

    private async Task AddCountries(FirmaContext context, int count)
    {
      for (int i = 0; i < count; i++)
      {
        string id = $"{i:D2}";
        context.Add(new Models.Drzava
        {
          OznDrzave = id,
          NazDrzave = id
        });
      }
      await context.SaveChangesAsync();
    }
  }
}
