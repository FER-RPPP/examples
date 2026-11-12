using Events.EF.Data;
using Events.EF.Models;
using Events.WebAPI.Models;
using Events.WebAPI.Util.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Events.WebAPI.Controllers;

/// <summary>
/// Web API service for sports CRUD operations
/// </summary>
[ApiController]
[Route("[controller]")]
[TypeFilter(typeof(ProblemDetailsForSqlException), Order = 10)]
[TypeFilter(typeof(ProblemDetailsForException), Order = 1)] //last one
public class SportsController : ControllerBase
{
  private readonly EventsContext ctx;

  public SportsController(EventsContext ctx)
  {
    this.ctx = ctx;
  }

  private static Expression<Func<Sport, SportDTO>> projection = c => new() 
  {
    Id = c.Id,
    Name = c.Name
  };

  /// <summary>
  /// returns number of sports satisfying filter (by sport name)
  /// </summary>
  /// <param name="filters">Optional filter with sport name (it looks for a substring inside the sport name)</param>
  /// <returns></returns>
  [HttpGet("count", Name = "SportsNumber")]
  public async Task<int> Count([FromQuery] string? filters)
  {
    var query = ctx.Sports.AsQueryable();
    if (!string.IsNullOrWhiteSpace(filters))
    {
      query = query.Where(m => m.Name.Contains(filters));
    }
    int count = await query.CountAsync(HttpContext.RequestAborted);
    return count;
  }

  /// <summary>
  /// Get all sports (optionally filtered by sport name)
  /// No of sports, order, and starting sport are determined using LoadParams
  /// </summary>
  /// <param name="loadParams">Paging and sorting parameters</param>
  /// <returns></returns>
  [HttpGet(Name = "GetSports")]
  public async Task<List<SportDTO>> GetAll([FromQuery] LoadParams loadParams) 
  {
    var query = ctx.Sports.AsQueryable();

    #region filtering, sorting, and paging
    if (!string.IsNullOrWhiteSpace(loadParams.Filters))
    {
      query = query.Where(m => m.Name.Contains(loadParams.Filters));
    }

    query = loadParams.Sort switch
    {
      nameof(SportDTO.Id) => loadParams.Ascending ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id),
      nameof(SportDTO.Name) => loadParams.Ascending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
      _ => query
    };

    if (loadParams.PageSize.HasValue)
    {
      query = query.Skip((loadParams.Page - 1) * loadParams.PageSize.Value)
                   .Take(loadParams.PageSize.Value);
    }
    #endregion  


    var list = await query.Select(projection)
                          .ToListAsync(HttpContext.RequestAborted);
    return list;
  }  

  /// <summary>
  /// Get sport by id 
  /// </summary>
  /// <param name="id">sport id</param>
  /// <returns>sport the internally has set id to the id from the route</returns>
  [HttpGet("{id}", Name = "GetSportById")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<SportDTO>> Get(int id)
  {   
    var sport = await ctx.Sports                            
                        .Where(m => m.Id == id)
                        .Select(projection)
                        .FirstOrDefaultAsync(HttpContext.RequestAborted);
    if (sport == null)
    {      
      return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No sport with id = {id}");
    }
    else
    {
      return sport;
    }
  }


  /// <summary>
  /// Delete sport base on its primary key
  /// </summary>
  /// <param name="id">Value of the primary key</param>
  /// <returns></returns>
  /// <response code="204">if the sport is deleted</response>
  /// <response code="404">if sport does not exists</response>      
  [HttpDelete("{id}", Name = "DeleteSport")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Delete(int id)
  {
    var sport = await ctx.Sports.FindAsync(id, HttpContext.RequestAborted);
    if (sport == null)
    {
      return NotFound();
    }
    else
    {
      ctx.Remove(sport);
      await ctx.SaveChangesAsync(HttpContext.RequestAborted);
      return NoContent();
    };     
  }

  /// <summary>
  /// Updates sport
  /// </summary>
  /// <param name="id">sport identifier (i.e., primary key)</param>
  /// <param name="model">Sport data. id from route, and id from the model must match</param>
  /// <returns></returns>
  [HttpPut("{id}", Name = "UpdateSport")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Update(int id, SportDTO model)
  {
    if (model.Id != id) //models are automatically validation because of [ApiController]
    {
      return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.Id}");
    }
    else
    {
      var sport = await ctx.Sports.FindAsync(id, HttpContext.RequestAborted);
      if (sport == null)
      { 
        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
      }

      sport.Name = model.Name;

      await ctx.SaveChangesAsync(HttpContext.RequestAborted);
      return NoContent();
    }
  }

  /// <summary>
  /// Adds new sport
  /// </summary>
  /// <param name="model">sport data</param>
  /// <returns></returns>
  [HttpPost(Name = "AddSport")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Create(SportDTO model)
  {
    Sport sport = new ()
    {
      Name = model.Name
    };
    ctx.Add(sport);
    await ctx.SaveChangesAsync(HttpContext.RequestAborted);

    var addedItem = await Get(sport.Id);

    return CreatedAtAction(nameof(Get), new { id = sport.Id }, addedItem.Value);
  }
}
