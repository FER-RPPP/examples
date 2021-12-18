using FluentAssertions;
using Microsoft.Playwright;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace MVC.UITests
{
  public class CitiesPagesShould 
  {
    private const string TestApp = "https://localhost:44389";

    [Trait("Category", "UI Tests")]
    [Fact]
    public async Task CanAddEditAndDeleteCity() 
    {      
      using var playwright = await Playwright.CreateAsync();
      await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
      {
        Headless = false,   
        SlowMo = 200
      });
      var context = await browser.NewContextAsync();
      // Open new page
      var page = await context.NewPageAsync();

      #region Browse from home to create page
      // Go to home page
      await page.GotoAsync($"{TestApp}");

      var locator = page.Locator("a:text(\"Mjesta\")");
      (await locator.CountAsync()).Should().BeGreaterThanOrEqualTo(1, because: "There must be menu for cities");
      
      // Click text=Mjesta
      await page.ClickAsync("text=Mjesta");
      Assert.Equal($"{TestApp}/Mjesto", page.Url);

      locator = page.Locator("a:text(\"Unos novog mjesta\")");
      (await locator.CountAsync()).Should().BeGreaterThanOrEqualTo(1, because: "There must be link to create page");
      await page.ClickAsync("text=Unos novog mjesta");
      Assert.Equal($"{TestApp}/Mjesto/Create", page.Url);
      #endregion

      #region Try to add empty city
      locator = page.Locator("button:text(\"Dodaj\")");
      (await locator.CountAsync()).Should().BeGreaterThanOrEqualTo(1, because: "There must be button to add");
      await page.ClickAsync("text=Dodaj");

      var validationErrors = await page.Locator(".validation-summary-errors ul li").CountAsync();
      validationErrors.Should().Be(3, because: "Empty data should cause validation to appear");
      #endregion

     #region Fill the test data and add it

      // Fill input[name="PostBrMjesta"]
      await page.FillAsync("input[name=\"PostBrMjesta\"]", "11");
      // Fill input[name="NazMjesta"]
      await page.FillAsync("input[name=\"NazMjesta\"]", "_demo");
      // Select _A2
      
      await page.SelectOptionAsync("select[name=\"OznDrzave\"]", new[] { "_A2" });
      await page.ClickAsync("input[name=\"PostBrMjesta\"]");

      await page.ClickAsync("text=Dodaj");
      Assert.Equal($"{ TestApp}/Mjesto", page.Url);
      #endregion

      #region Get id of the added city
      locator = page.Locator("div:text(\"Mjesto _demo dodano\")");
      (await locator.CountAsync()).Should().BeGreaterThanOrEqualTo(1, because: "There should be message upon succesfull add");

      var content = await locator.TextContentAsync();
      int id = int.Parse(Regex.Replace(content, @"\D", ""));
      #endregion
     
      #region Edit city
      // Click edit
      await page.ClickAsync($"a[data-get-ajax='/Mjesto/GetAjax/{id}']");
      // Click input[name="PostBrMjesta"]
      await page.ClickAsync("input[name=\"PostBrMjesta\"]");
      // Fill input[name="PostBrMjesta"]
      await page.FillAsync("input[name=\"PostBrMjesta\"]", "12");
      // Click input[name="NazMjesta"]
      await page.ClickAsync("input[name=\"NazMjesta\"]");
      // Fill input[name="NazMjesta"]
      await page.FillAsync("input[name=\"NazMjesta\"]", "_demox");
      await page.ClickAsync(".save");
      #endregion

      #region Delete city
      void page_Dialog1_EventHandler(object sender, IDialog dialog)
      {
        Console.WriteLine($"Dialog message: {dialog.Message}");
        dialog.AcceptAsync();
        page.Dialog -= page_Dialog1_EventHandler;
      }
      page.Dialog += page_Dialog1_EventHandler;
      await page.ClickAsync($".delete[data-delete-ajax='/Mjesto/DeleteAjax/{id}']");
      #endregion
    }
  }
}
