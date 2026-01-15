using EFModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApi.Models;
using WebServices.Util.ExceptionFilters;

namespace WebApi.Controllers;

/// <summary>
/// Web API service for cities CRUD operations
/// </summary>
[ApiController]
[Route("[controller]")]
[TypeFilter(typeof(ProblemDetailsForSqlException))]
public class CitiesController : ControllerBase, ICustomController<int, CityViewModel>
{
  private readonly FirmContext ctx;
  private static Dictionary<string, Expression<Func<City, object?>>> orderSelectors = new()
  {
    [nameof(CityViewModel.CityId).ToLower()] = c => c.CityId,
    [nameof(CityViewModel.CountryName).ToLower()] = c => c.CountryCodeNavigation.CountryName,
    [nameof(CityViewModel.CityName).ToLower()] = c => c.CityName,
    [nameof(CityViewModel.CountryCode).ToLower()] = c => c.CountryCode,
    [nameof(CityViewModel.PostalCode).ToLower()] = c => c.PostalCode,
    [nameof(CityViewModel.PostalName).ToLower()] = c => c.PostalName
  };

  private static Expression<Func<City, CityViewModel>> projection = c => new CityViewModel
  {
    CityId = c.CityId,
    CountryName = c.CountryCodeNavigation.CountryName,
    CityName = c.CityName,
    CountryCode = c.CountryCode,
    PostalCode = c.PostalCode,
    PostalName = c.PostalName
  };

  public CitiesController(FirmContext ctx) 
  {
    this.ctx = ctx;
  }


  /// <summary>
  /// returns number of cities satisfying filter (by city name)
  /// </summary>
  /// <param name="filter">Optional filter for city name</param>
  /// <returns></returns>
  [HttpGet("count", Name = "CitiesNumber")]
  public async Task<int> Count([FromQuery] string? filter)
  {
    var query = ctx.Cities.AsQueryable();
    if (!string.IsNullOrWhiteSpace(filter))
    {
      query = query.Where(m => m.CityName.Contains(filter));
    }
    int count = await query.CountAsync();
    return count;
  }

  /// <summary>
  /// Get all cities (optionally filtered by city name)
  /// No of cities, order, and starting city are determined using LoadParams
  /// </summary>
  /// <param name="loadParams">Paging and sortin parameters</param>
  /// <returns></returns>
  [HttpGet(Name = "GetCities")]
  public async Task<List<CityViewModel>> GetAll([FromQuery] LoadParams loadParams) 
  {
    var query = ctx.Cities.AsQueryable();

    if (!string.IsNullOrWhiteSpace(loadParams.Filter))
    {
      query = query.Where(m => m.CityName.Contains(loadParams.Filter));
    }

    if (loadParams.SortColumn != null)
    {
      if (orderSelectors.TryGetValue(loadParams.SortColumn.ToLower(), out var expr))
      {
        query = loadParams.Descending ? query.OrderByDescending(expr) : query.OrderBy(expr);
      }
    }

    var list = await query.Select(projection)
                          .Skip(loadParams.StartIndex)
                          .Take(loadParams.Rows)
                          .ToListAsync();
    return list;
  }  

  /// <summary>
  /// Get city by id (note: this is not postal code, but it's primary key)
  /// </summary>
  /// <param name="id">city id</param>
  /// <returns>city the internally has set id to the id from the route</returns>
  [HttpGet("{id}", Name = "GetCityById")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<CityViewModel>> Get(int id)
  {   
    var city = await ctx.Cities                            
                        .Where(m => m.CityId == id)
                        .Select(projection)
                        .FirstOrDefaultAsync();
    if (city == null)
    {      
      return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for id = {id}");
    }
    else
    {
      return city;
    }
  }


  /// <summary>
  /// Delete city base on its primary key
  /// </summary>
  /// <param name="id">Value of the primary key (i.e., CityId)</param>
  /// <returns></returns>
  /// <response code="204">if the city is deleted</response>
  /// <response code="404">if city does not exists</response>      
  [HttpDelete("{id}", Name = "DeleteCity")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Delete(int id)
  {
    var city = await ctx.Cities.FindAsync(id);
    if (city == null)
    {
      return NotFound();
    }
    else
    {
      ctx.Remove(city);
      await ctx.SaveChangesAsync();
      return NoContent();
    };     
  }

  /// <summary>
  /// Updates city
  /// </summary>
  /// <param name="id">city identifier (i.e., primary key)</param>
  /// <param name="model">City data. id from route, and id from the model must match</param>
  /// <returns></returns>
  [HttpPut("{id}", Name = "UpdateCity")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Update(int id, CityViewModel model)
  {
    if (model.CityId != id) //ModelState.IsValid i model != null are automatically evalued because of [ApiController]
    {
      return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.CityId}");
    }
    else
    {
      var city = await ctx.Cities.FindAsync(id);
      if (city == null)
      { 
        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
      }

      city.CityName = model.CityName;
      city.CountryCode = model.CountryCode;
      city.PostalCode = model.PostalCode;
      city.PostalName = model.PostalName;

      await ctx.SaveChangesAsync();
      return NoContent();
    }
  }

  /// <summary>
  /// Adds new city
  /// </summary>
  /// <param name="model">city data</param>
  /// <returns></returns>
  [HttpPost(Name = "AddCity")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Create(CityViewModel model)
  {
    City city = new ()
    {
      CityName = model.CityName,
      PostalCode = model.PostalCode,
      CountryCode = model.CountryCode,
      PostalName = model.PostalName
    };
    ctx.Add(city);
    await ctx.SaveChangesAsync();

    var addedItem = await Get(city.CityId);

    return CreatedAtAction(nameof(Get), new { id = city.CityId }, addedItem.Value);
  }
}
