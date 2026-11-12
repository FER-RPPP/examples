using Events.EF.Data;
using Events.EF.Models;
using Events.WebAPI.Models;
using Events.WebAPI.Util.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System.Linq.Expressions;

namespace Events.WebAPI.Controllers;

/// <summary>
/// Web API service for registrations CRUD operations
/// </summary>
[ApiController]
[Route("[controller]")]
[TypeFilter(typeof(ProblemDetailsForSqlException), Order = 10)]
[TypeFilter(typeof(ProblemDetailsForException), Order = 1)] //last one
public class RegistrationsController : ControllerBase
{
  private readonly EventsContext ctx;
  private readonly ISieveProcessor sieveProcessor;

  public RegistrationsController(EventsContext ctx, ISieveProcessor sieveProcessor)
  {
    this.ctx = ctx;
    this.sieveProcessor = sieveProcessor;
  }

  private static Expression<Func<Registration, RegistrationDTO>> projection = c => new() 
  {
    Id = c.Id,
    EventId = c.EventId,
    PersonId = c.PersonId,
    SportId = c.SportId,
    RegisteredAt = c.RegisteredAt,
    EventName = c.Event.Name,
    PersonName = c.Person.FirstName + " " + c.Person.LastName,
    PersonFirstNameTranscription = c.Person.FirstNameTranscription,
    PersonLastNameTranscription = c.Person.LastNameTranscription,
    SportName = c.Sport.Name,
    CountryName = c.Person.CountryCodeNavigation.Name,
    CountryCode = c.Person.CountryCode
  };

  
  /// <summary>
  /// returns number of registrations satisfying filter (by registration details)
  /// </summary>
  /// <param name="filters">Optional Sieve filter for registration</param>
  /// <returns></returns>
  [HttpGet("count", Name = "RegistrationsNumber")]
  public async Task<int> Count([FromQuery] string? filters)
  {
    var query = ctx.Registrations.Select(projection);
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
  /// Get all registrations (optionally filtered using Sieve filter syntax)
  /// No of registrations, order, and starting registration are determined using LoadParams
  /// </summary>
  /// <param name="loadParams">Paging and sorting parameters</param>
  /// <returns></returns>
  [HttpGet(Name = "GetRegistrations")]
  public async Task<List<RegistrationDTO>> GetAll([FromQuery] LoadParams loadParams) 
  {
    var query = ctx.Registrations.Select(projection);

    SieveModel sieveModel = new SieveModel()
    {
      Filters = loadParams.Filters,
      Sorts = loadParams.GetSort(nameof(RegistrationDTO.RegisteredAt)),
      PageSize = loadParams.PageSize,
      Page = loadParams.Page
    };

    var filteredQuery = sieveProcessor.Apply(sieveModel, query);

    var list = await filteredQuery.ToListAsync(HttpContext.RequestAborted);
    return list;
  }  

  /// <summary>
  /// Get registration by id 
  /// </summary>
  /// <param name="id">registration id</param>
  /// <returns>registration the internally has set id to the id from the route</returns>
  [HttpGet("{id}", Name = "GetRegistrationById")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<RegistrationDTO>> Get(int id)
  {   
    var @registration = await ctx.Registrations                            
                        .Where(m => m.Id == id)
                        .Select(projection)
                        .FirstOrDefaultAsync(HttpContext.RequestAborted);
    if (registration == null)
    {      
      return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No registration with id = {id}");
    }
    else
    {
      return registration;
    }
  }


  /// <summary>
  /// Delete registration base on its primary key
  /// </summary>
  /// <param name="id">Value of the primary key</param>
  /// <returns></returns>
  /// <response code="204">if the registration is deleted</response>
  /// <response code="404">if registration does not exists</response>      
  [HttpDelete("{id}", Name = "DeleteRegistration")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Delete(int id)
  {
    var registration = await ctx.Registrations.FindAsync(id, HttpContext.RequestAborted);
    if (registration == null)
    {
      //return NotFound();
      return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
    }
    else
    {
      ctx.Remove(registration);
      await ctx.SaveChangesAsync(HttpContext.RequestAborted);
      return NoContent();
    };     
  }

  /// <summary>
  /// Updates registration
  /// </summary>
  /// <param name="id">registration identifier (i.e., primary key)</param>
  /// <param name="model">Registration data. id from route, and id from the model must match</param>
  /// <param name="validator"></param>
  /// <returns></returns>
  [HttpPut("{id}", Name = "UpdateRegistration")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Update(int id, RegistrationDTO model, [FromServices] IValidator<RegistrationDTO> validator)
  {
    if (model.Id != id) //models are automatically validation because of [ApiController]
    {
      return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.Id}");
    }
    else
    {
      var registration = await ctx.Registrations.FindAsync(id, HttpContext.RequestAborted);
      if (registration == null)
      { 
        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
      }

      var validationResult = await validator.ValidateAsync(model, HttpContext.RequestAborted);
      if (!validationResult.IsValid)
      {
        return ValidationProblem(
          new ValidationProblemDetails(validationResult.ToDictionary()));
      }

      registration.SportId = model.SportId;
      registration.PersonId = model.PersonId; 
      registration.EventId = model.EventId;

      await ctx.SaveChangesAsync(HttpContext.RequestAborted);
      return NoContent();
    }
  }

  /// <summary>
  /// Adds new registration
  /// </summary>
  /// <param name="model">registration data</param>
  /// <param name="validator"></param>
  /// <returns></returns>
  [HttpPost(Name = "AddRegistration")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Create(RegistrationDTO model, [FromServices] IValidator<RegistrationDTO> validator)
  {
    var validationResult = await validator.ValidateAsync(model, HttpContext.RequestAborted);
    if (!validationResult.IsValid)
    {
      return ValidationProblem(
        new ValidationProblemDetails(validationResult.ToDictionary()));
    }

    Registration registration = new () { 
      SportId = model.SportId,
      PersonId = model.PersonId,
      EventId = model.EventId
    };
    ctx.Add(registration);
    await ctx.SaveChangesAsync(HttpContext.RequestAborted);

    var addedItem = await Get(registration.Id);

    return CreatedAtAction(nameof(Get), new { id = registration.Id }, addedItem.Value);
  }
}
