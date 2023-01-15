using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using MVC_EN;
using MVC_EN.Controllers;
using MVC_EN.Models;

namespace MVC.UnitTests.Controllers.Cities
{
  public class Create
  {
    private readonly IOptionsSnapshot<AppSettings> options;        

    public Create()
    {      
      var mockOptions = new Mock<IOptionsSnapshot<AppSettings>>();
      var appSettings = new AppSettings
      {
        PageSize = 10
      };
      mockOptions.SetupGet(options => options.Value)
                 .Returns(appSettings);
      options = mockOptions.Object;
    }

    [Fact]
    [Trait("Category", "CitiesController")]
    public async Task PreparesViewBag()
    {
      int totalNumber = 50;
      // Arrange            
      var dbOptions = new DbContextOptionsBuilder<FirmContext>()
                          .UseInMemoryDatabase(databaseName: nameof(PreparesViewBag))
                          .Options;

      using var context = new FirmContext(dbOptions);
      await AddCountries(context, totalNumber);

      var controller = new CitiesController(context, options);      

      // Act
      var result = controller.Create();

      // Assert
      var task = Assert.IsType<Task<IActionResult>>(result);
      IActionResult actionResult = await task;
      var viewResult = Assert.IsType<ViewResult>(actionResult);
      viewResult.Model.Should().BeNull();

      var countries = viewResult.ViewData["Countries"];
      countries.Should()
               .NotBeNull(because: "controller should prepare data for drop down list")
               .And.BeOfType<SelectList>()
               .Which.Count().Should().Be(totalNumber);      
    }

    //Think about why this test fails!!
    [Fact]
    [Trait("Category", "CitiesController")]
    public async Task CauseValidationErrorForEmptyCity()
    {      
      // Arrange            
      var dbOptions = new DbContextOptionsBuilder<FirmContext>()
                          .UseInMemoryDatabase(databaseName: nameof(CauseValidationErrorForEmptyCity))
                          .Options;

      using var context = new FirmContext(dbOptions);      
      var controller = new CitiesController(context, options);

      // Act
      var result = controller.Create(new City());

      // Assert
      context.Cities.Count().Should().Be(0, because: "city without data should not be added in a database");
      var task = Assert.IsType<Task<IActionResult>>(result);
      IActionResult actionResult = await task;

      actionResult.Should().BeOfType<ViewResult>()
                  .Which.Model.Should().NotBeNull();

      controller.ModelState.Should().NotBeNull(because: "city without data should have validation errors")
                .And.ContainKeys(
                  nameof(City.PostalCode),
                  nameof(City.CityName),
                  nameof(City.CountryCode)
                );      
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
