using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MVC_EN.Extensions;
using MVC_EN.Extensions.Selectors;
using MVC_EN.Models;
using MVC_EN.ViewModels;
using System.Text.Json;

namespace MVC_EN.Controllers;

public class CountriesController : Controller
{
  private readonly FirmContext ctx;
  private readonly ILogger<CountriesController> logger;
  private readonly AppSettings appSettings;

  public CountriesController(FirmContext ctx, IOptionsSnapshot<AppSettings> options, ILogger<CountriesController> logger)
  {
    this.ctx = ctx;
    this.logger = logger;
    appSettings = options.Value;
  }

  //public IActionResult Index()
  //{
  //  var countries = ctx.Countries
  //                  .AsNoTracking()
  //                  .OrderBy(d => d.CountryName)
  //                  .ToList();
  //  return View("IndexSimple", countries);
  //}

  public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
  {
    int pagesize = appSettings.PageSize;

    IQueryable<Country> query = ctx.Countries
                                   .AsNoTracking();

    int count = query.Count();
    if (count == 0)
    {
      string message = "There is no country in the database";
      logger.LogInformation(message);
      TempData[Constants.Message] = "message";
      TempData[Constants.ErrorOccurred] = false;
      return RedirectToAction(nameof(Create));
    }

    var pagingInfo = new PagingInfo
    {
      CurrentPage = page,
      Sort = sort,
      Ascending = ascending,
      ItemsPerPage = pagesize,
      TotalItems = count
    };
    if (page < 1 || page > pagingInfo.TotalPages)
    {
      return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
    }

    query = query.ApplySort(sort, ascending);

    var model = PagedList<Country>.Create(query, pagingInfo);

    return View(model);
  }

  [HttpGet]
  public IActionResult Create()
  {
    return View();
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult Create(Country country)
  {
    logger.LogTrace(JsonSerializer.Serialize(country));
    if (ModelState.IsValid)
    {
      try
      {
        ctx.Add(country);
        ctx.SaveChanges();
        string message = $"Country {country.CountryName} added.";
        logger.LogInformation(new EventId(1000), message);

        TempData[Constants.Message] = message;
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Index));
      }
      catch (Exception exc)
      {
        logger.LogError("Error adding new country: {0}", exc.CompleteExceptionMessage());
        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
        return View(country);
      }
    }
    else
    {
      return View(country);
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public IActionResult Delete(string countryCode, int page = 1, int sort = 1, bool ascending = true)
  {
    var country = ctx.Countries.Find(countryCode);                       
    if (country != null)
    {
      try
      {
        string naziv = country.CountryName;
        ctx.Remove(country);
        ctx.SaveChanges();
        string message = $"Country {naziv} deleted";
        logger.LogInformation(message);
        TempData[Constants.Message] = message;
        TempData[Constants.ErrorOccurred] = false;
      }
      catch (Exception exc)
      {
        string message = "Eror deleting the country " + exc.CompleteExceptionMessage();
        TempData[Constants.Message] = message;
        TempData[Constants.ErrorOccurred] = true;
        logger.LogError(message);
      }
    }
    else
    {
      string message = $"There is no country with code {countryCode}";
      logger.LogWarning(message);
      TempData[Constants.Message] = message;
      TempData[Constants.ErrorOccurred] = true;
    }
    return RedirectToAction(nameof(Index), new { page, sort, ascending });
  }

  [HttpGet]
  public IActionResult Edit(string id, int page = 1, int sort = 1, bool ascending = true)
  {
    var country = ctx.Countries.AsNoTracking().Where(d => d.CountryCode == id).SingleOrDefault();
    if (country == null)
    {
      string message = $"There is no country with code {id}";
      logger.LogWarning(message);
      return NotFound(message);
    }
    else
    {
      ViewBag.Page = page;
      ViewBag.Sort = sort;
      ViewBag.Ascending = ascending;
      return View(country);
    }
  }

  [HttpPost, ActionName("Edit")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Update(string id, int page = 1, int sort = 1, bool ascending = true)
  {
    //for different approaches (attach, update, only id) see
    //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

    try
    {
      Country? country = await ctx.Countries.FindAsync(id);
      if (country == null)
      {
        return NotFound("Invalid country code: " + id);
      }

      if (await TryUpdateModelAsync<Country>(country, "",
          d => d.CountryName, d => d.CountryIso3
      ))
      {
        ViewBag.Page = page;
        ViewBag.Sort = sort;
        ViewBag.Ascending = ascending;
        try
        {
          await ctx.SaveChangesAsync();
          TempData[Constants.Message] = "Country updated.";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index), new { page, sort, ascending });
        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          return View(country);
        }
      }
      else
      {
        ModelState.AddModelError(string.Empty, "Cannot update model");
        return View(country);
      }
    }
    catch (Exception exc)
    {
      TempData[Constants.Message] = exc.CompleteExceptionMessage();
      TempData[Constants.ErrorOccurred] = true;
      return RedirectToAction(nameof(Edit), id);
    }
  }
}
