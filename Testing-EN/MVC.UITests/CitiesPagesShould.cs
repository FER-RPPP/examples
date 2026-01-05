using FluentAssertions;
using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace MVC.UITests;

public class CitiesPagesShould 
{
  private const string TestApp = "https://localhost:54145";

  [Trait("Category", "UI Tests")]
  [Fact]
  public async Task CanAddEditAndDeleteCity() 
  {      
    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
      Headless = false,   
      SlowMo = 2000
    });
    var context = await browser.NewContextAsync();
    // Open new page
    var page = await context.NewPageAsync();

  
    #region Browse from home to create page
    // Go to home page
    await page.GotoAsync($"{TestApp}");

    var locator = page.GetByRole(AriaRole.Link, new() { Name = "Cities" });
    (await locator.CountAsync()).Should().BeGreaterThanOrEqualTo(1, because: "There must be menu for cities");

    // Click text=Cities
    await locator.ClickAsync();
    Assert.Equal($"{TestApp}/Cities", page.Url);

    locator = page.GetByRole(AriaRole.Link, new() { Name = "Add a new city" });
    (await locator.CountAsync()).Should().BeGreaterThanOrEqualTo(1, because: "There must be link to create new city");
    await locator.ClickAsync();
    Assert.Equal($"{TestApp}/Cities/Create", page.Url);
    #endregion

    #region Try to add empty city
    locator = page.GetByRole(AriaRole.Button, new() { Name = "Add" });
    (await locator.CountAsync()).Should().BeGreaterThanOrEqualTo(1, because: "There must be button to add");
    await locator.ClickAsync();

    var validationErrors = await page.Locator(".validation-summary-errors ul li").CountAsync();
    validationErrors.Should().Be(3, because: "Empty data should cause validation to appear");
    #endregion

    #region Fill the test data and add it

    // Fill input[name="PostalCode"]
    await page.GetByLabel("Postal Code").FillAsync("11");
    // Fill input[name="CityName"]
    await page.GetByLabel("City Name").FillAsync("_demo");
    // Select _A2

    var combo = page.GetByRole(AriaRole.Combobox, new() { Name = "Country" });
    await combo.SelectOptionAsync(new[] { "_A2" });
    await combo.PressAsync("Tab"); //to lose the focus and trigger validation (i.e. remove previous validation errors)

    await page.GetByRole(AriaRole.Button, new() { Name = "Add" }).ClickAsync();
    Assert.Equal($"{TestApp}/Cities", page.Url);
    #endregion

    #region Get id of the added city
    locator = page.GetByText(new Regex("^City _demo has been added"));
    (await locator.CountAsync()).Should().BeGreaterThanOrEqualTo(1, because: "There should be message upon succesfull add");

    var content = await locator.TextContentAsync();
    int id = int.Parse(Regex.Replace(content, @"\D", ""));
    #endregion
   
    #region Edit city
    // Click edit
    await page.ClickAsync($"a[hx-get='/Cities/Edit/{id}']");
    // Click input[name="PostalCode"]
    await page.ClickAsync("input[name=\"PostalCode\"]");
    // Fill input[name="PostalCode"]
    await page.FillAsync("input[name=\"PostalCode\"]", "12");
    // Click input[name="CityName"]
    await page.ClickAsync("input[name=\"CityName\"]");
    // Fill input[name="CityName"]
    await page.FillAsync("input[name=\"CityName\"]", "_demox");
    await page.ClickAsync($"button[hx-post='/Cities/Edit/{id}']");
    #endregion

    #region Delete city
    void page_Dialog1_EventHandler(object sender, IDialog dialog)
    {
      Console.WriteLine($"Dialog message: {dialog.Message}");
      dialog.AcceptAsync();
      page.Dialog -= page_Dialog1_EventHandler;
    }
    page.Dialog += page_Dialog1_EventHandler;
    await page.ClickAsync($"button[hx-delete='/Cities/Delete/{id}']");
    #endregion
  }
}
