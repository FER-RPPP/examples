using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MVC.Controllers;
using MVC.Models;
using MVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MVC.UnitTests.Controllers.Mjesto
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
      var dbOptions = new DbContextOptionsBuilder<FirmaContext>()
                          .UseInMemoryDatabase(databaseName: nameof(PreparesViewBag))
                          .Options;

      using var context = new FirmaContext(dbOptions);
      await AddCountries(context, totalNumber);

      var controller = new MjestoController(context, options);      

      // Act
      var result = controller.Create();

      // Assert
      var task = Assert.IsType<Task<IActionResult>>(result);
      IActionResult actionResult = await task;
      var viewResult = Assert.IsType<ViewResult>(actionResult);
      viewResult.Model.Should().BeNull();

      var countries = viewResult.ViewData["Drzave"];
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
      var dbOptions = new DbContextOptionsBuilder<FirmaContext>()
                          .UseInMemoryDatabase(databaseName: nameof(CauseValidationErrorForEmptyCity))
                          .Options;

      using var context = new FirmaContext(dbOptions);      
      var controller = new MjestoController(context, options);

      // Act
      var result = controller.Create(new Models.Mjesto());

      // Assert
      context.Mjesto.Count().Should().Be(0, because: "city without data should not be added in a database");
      var task = Assert.IsType<Task<IActionResult>>(result);
      IActionResult actionResult = await task;

      actionResult.Should().BeOfType<ViewResult>()
                  .Which.Model.Should().NotBeNull();

      controller.ModelState.Should().NotBeNull(because: "city without data should have validation errors")
                .And.ContainKeys(
                  nameof(Models.Mjesto.PostBrMjesta),
                  nameof(Models.Mjesto.NazMjesta),
                  nameof(Models.Mjesto.OznDrzave)
                );      
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
