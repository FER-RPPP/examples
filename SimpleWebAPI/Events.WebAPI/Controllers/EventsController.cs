using Events.EF.Data;
using Events.EF.Models;
using Events.WebAPI.Models;
using Events.WebAPI.Util.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System.Linq.Expressions;

namespace Events.WebAPI.Controllers;

/// <summary>
/// Web API service for events CRUD operations
/// </summary>
[ApiController]
[Route("[controller]")]
[TypeFilter(typeof(ProblemDetailsForSqlException), Order = 10)]
[TypeFilter(typeof(ProblemDetailsForException), Order = 1)] //last one
public class EventsController : ControllerBase
{
  private readonly EventsContext ctx;
  private readonly ISieveProcessor sieveProcessor;

  public EventsController(EventsContext ctx, ISieveProcessor sieveProcessor)
  {
    this.ctx = ctx;
    this.sieveProcessor = sieveProcessor;
  }

  private static Expression<Func<Event, EventDTO>> projection = c => new() 
  {
    Id = c.Id,
    Name = c.Name,
    EventDate = c.EventDate,
    RegistrationsCount = c.Registrations.Count
  };

  
  /// <summary>
  /// returns number of events satisfying filter (by event name)
  /// </summary>
  /// <param name="filters">Optional Sieve filter for event</param>
  /// <returns></returns>
  [HttpGet("count", Name = "EventsNumber")]
  public async Task<int> Count([FromQuery] string? filters)
  {
    var query = ctx.Events.Select(projection);
    int count;
    if (string.IsNullOrWhiteSpace(filters))
    {
      count = await query.CountAsync(HttpContext.RequestAborted);
    }
    else
    {
      SieveModel sieveModel = new SieveModel()
      {
        Filters = filters
      };
      var filteredQuery = sieveProcessor.Apply(sieveModel, query, applyFiltering: true, applySorting: false, applyPagination: false);
      count = await filteredQuery.CountAsync(HttpContext.RequestAborted);
    }
    return count;
  }

  /// <summary>
  /// Get all events (optionally filtered using Sieve filter syntax)
  /// No of events, order, and starting event are determined using LoadParams
  /// </summary>
  /// <param name="loadParams">Paging and sorting parameters</param>
  /// <returns></returns>
  [HttpGet(Name = "GetEvents")]
  public async Task<List<EventDTO>> GetAll([FromQuery] LoadParams loadParams) 
  {
    var query = ctx.Events.Select(projection);

    SieveModel sieveModel = new SieveModel()
    {
      Filters = loadParams.Filters,
      Sorts = loadParams.GetSort(nameof(EventDTO.EventDate)),
      PageSize = loadParams.PageSize,
      Page = loadParams.Page
    };

    var filteredQuery = sieveProcessor.Apply(sieveModel, query);

    var list = await filteredQuery.ToListAsync(HttpContext.RequestAborted);
    return list;
  }  

  /// <summary>
  /// Get event by id 
  /// </summary>
  /// <param name="id">event id</param>
  /// <returns>event the internally has set id to the id from the route</returns>
  [HttpGet("{id}", Name = "GetEventById")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<EventDTO>> Get(int id)
  {   
    var @event = await ctx.Events                            
                        .Where(m => m.Id == id)
                        .Select(projection)
                        .FirstOrDefaultAsync(HttpContext.RequestAborted);
    if (@event == null)
    {      
      return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No event with id = {id}");
    }
    else
    {
      return @event;
    }
  }


  /// <summary>
  /// Delete event base on its primary key
  /// </summary>
  /// <param name="id">Value of the primary key</param>
  /// <returns></returns>
  /// <response code="204">if the event is deleted</response>
  /// <response code="404">if event does not exists</response>      
  [HttpDelete("{id}", Name = "DeleteEvent")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Delete(int id)
  {
    var @event = await ctx.Events.FindAsync(id, HttpContext.RequestAborted);
    if (@event == null)
    {
      return NotFound();
    }
    else
    {
      ctx.Remove(@event);
      await ctx.SaveChangesAsync(HttpContext.RequestAborted);
      return NoContent();
    };     
  }

  /// <summary>
  /// Updates event
  /// </summary>
  /// <param name="id">event identifier (i.e., primary key)</param>
  /// <param name="model">Event data. id from route, and id from the model must match</param>
  /// <returns></returns>
  [HttpPut("{id}", Name = "UpdateEvent")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Update(int id, EventDTO model)
  {
    if (model.Id != id) //models are automatically validation because of [ApiController]
    {
      return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.Id}");
    }
    else
    {
      var @event = await ctx.Events.FindAsync(id, HttpContext.RequestAborted);
      if (@event == null)
      { 
        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
      }

      @event.Name = model.Name;
      @event.EventDate = model.EventDate;

      await ctx.SaveChangesAsync(HttpContext.RequestAborted);
      return NoContent();
    }
  }

  /// <summary>
  /// Adds new event
  /// </summary>
  /// <param name="model">event data</param>
  /// <returns></returns>
  [HttpPost(Name = "AddEvent")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Create(EventDTO model)
  {
    Event @event = new ()
    {
      Name = model.Name,
      EventDate = model.EventDate
    };
    ctx.Add(@event);
    await ctx.SaveChangesAsync(HttpContext.RequestAborted);

    var addedItem = await Get(@event.Id);

    return CreatedAtAction(nameof(Get), new { id = @event.Id }, addedItem.Value);
  }
}
