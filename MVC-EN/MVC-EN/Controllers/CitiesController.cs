using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MVC_EN.Extensions;
using MVC_EN.Extensions.Selectors;
using MVC_EN.Models;
using MVC_EN.ViewModels;

namespace MVC_EN.Controllers;
public class CitiesController : Controller
{
  private readonly FirmContext ctx;
  private readonly AppSettings appData;

  private static Expression<Func<City, CityViewModel>> selectProjection = c => new CityViewModel
  {
    CityId = c.CityId,
    CityName = c.CityName,
    PostalName = c.PostalName,
    PostalCode = c.PostalCode,
    CountryCode = c.CountryCode,
    CountryName = c.CountryCodeNavigation.CountryName
  };

  public CitiesController(FirmContext ctx, IOptionsSnapshot<AppSettings> options)
  {
    this.ctx = ctx;
    appData = options.Value;
  }

  public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
  {
    int pagesize = appData.PageSize;

    var query = ctx.Cities.AsNoTracking();
    int count = await query.CountAsync();

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
   
    var cities = query.Select(selectProjection);
    var model = await PagedList<CityViewModel>.CreateAsync(cities, pagingInfo);

    return View(model);
  }   

  [HttpGet]
  public async Task<IActionResult> Create()
  {
    await PrepareDropDownLists();
    return View();
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(CityViewModel model)
  {
    if (ModelState.IsValid)
    {
      try
      {
        var city = new City
        {
          CityName = model.CityName,
          PostalName = model.PostalName, 
          PostalCode = model.PostalCode,
          CountryCode = model.CountryCode
        };
        ctx.Add(city);
        await ctx.SaveChangesAsync();

        TempData[Constants.Message] = $"City {model.CityName} has been added with id = {city.CityId}";
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Index));

      }
      catch (Exception exc)
      {
        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
        await PrepareDropDownLists();
        return View(model);
      }
    }
    else
    {
      await PrepareDropDownLists();
      return View(model);
    }
  }

  private async Task PrepareDropDownLists()
  {
    var hr = await ctx.Countries                  
                      .Where(d => d.CountryCode == "HR")
                      .Select(d => new { d.CountryName, d.CountryCode })
                      .FirstOrDefaultAsync();
    var countries = await ctx.Countries
                          .Where(d => d.CountryCode != "HR")
                          .OrderBy(d => d.CountryName)
                          .Select(d => new { d.CountryName, d.CountryCode })
                          .ToListAsync();
    if (hr != null)
    {
      countries.Insert(0, hr);
    }      
    ViewBag.Countries = new SelectList(countries, nameof(hr.CountryCode), nameof(hr.CountryName));
  }
  

  #region Methods for dynamic update and delete
  [HttpDelete]
  public async Task<IActionResult> Delete(int id)
  {
    ActionResponseMessage responseMessage;
    var city = await ctx.Cities.FindAsync(id);          
    if (city != null)
    {
      try
      {
        string name = city.CityName;
        ctx.Remove(city);
        await ctx.SaveChangesAsync();
        responseMessage = new ActionResponseMessage(MessageType.Success, $"City {name} with id {id} has been deleted.");          
      }
      catch (Exception exc)
      {          
        responseMessage = new ActionResponseMessage(MessageType.Error, $"Error deleting city: {exc.CompleteExceptionMessage()}");
      }
    }
    else
    {
      responseMessage = new ActionResponseMessage(MessageType.Error, $"City with id {id} does not exist.");
    }

    Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
    return responseMessage.MessageType == MessageType.Success ?
      new EmptyResult() : await Get(id);
  }

  [HttpGet]
  public async Task<IActionResult> Edit(int id)
  {
    var city = await ctx.Cities
                          .Where(m => m.CityId == id)
                          .Select(selectProjection)
                          .SingleOrDefaultAsync();
    if (city != null)
    {        
      await PrepareDropDownLists();
      return PartialView(city);
    }
    else
    {
      return NotFound($"Invalid city id: {id}");
    }
  }

  [HttpPost]    
  public async Task<IActionResult> Edit(CityViewModel model)
  {
    if (model == null)
    {
      return NotFound("No data submitted!?");
    }  

    if (ModelState.IsValid)
    {
      try
      {
        var city = await ctx.Cities.FindAsync(model.CityId);
        if (city == null)
        {
          return NotFound($"Invalid city id: {model.CityId}");
        }
        city.CityName = model.CityName;
        city.PostalName = model.PostalName;
        city.PostalCode = model.PostalCode;
        city.CountryCode = model.CountryCode;

        await ctx.SaveChangesAsync();
        return RedirectToAction(nameof(Get), new { id = model.CityId });
      }
      catch (Exception exc)
      {
        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
        await PrepareDropDownLists();
        return PartialView(model);
      }
    }
    else
    {
      await PrepareDropDownLists();
      return PartialView(model);
    }
  }

  [HttpGet]
  public async Task<IActionResult> Get(int id)
  {
    var city = await ctx.Cities
                        .Where(m => m.CityId == id)
                        .Select(selectProjection)
                        .SingleOrDefaultAsync();
    if (city != null)
    {
      return PartialView(city);
    }
    else
    {
      return NotFound($"Invalid city id: {id}");
    }
  }
  #endregion
}
