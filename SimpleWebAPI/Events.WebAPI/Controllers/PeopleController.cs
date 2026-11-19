using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Events.EF.Data;
using Events.EF.Models;
using Events.WebAPI.Models;
using Events.WebAPI.Util.Mappers;
using Events.WebAPI.Util.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace Events.WebAPI.Controllers;

/// <summary>
/// Web API service for registrations CRUD operations
/// </summary>
[ApiController]
[Route("[controller]")]
[TypeFilter(typeof(ProblemDetailsForSqlException), Order = 10)]
[TypeFilter(typeof(ProblemDetailsForException), Order = 1)] //last one
public class PeopleController : ControllerBase
{
  private readonly EventsContext ctx;
  private readonly ISieveProcessor sieveProcessor;

  public PeopleController(EventsContext ctx, ISieveProcessor sieveProcessor)
  {
    this.ctx = ctx;
    this.sieveProcessor = sieveProcessor;
  }
  
  /// <summary>
  /// returns number of people satisfying filter (by registration details)
  /// </summary>
  /// <param name="filters">Optional Sieve filter for registration</param>
  /// <returns></returns>
  [HttpGet("count", Name = "PeopleNumber")]
  public async Task<int> Count([FromQuery] string? filters)
  {    
    int count;
    if (string.IsNullOrWhiteSpace(filters))
    {
      count = await ctx.People.CountAsync(HttpContext.RequestAborted);
    }
    else
    {
      var query = ctx.People.ProjectToDTO();
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
  /// Get all people (optionally filtered using Sieve filter syntax)
  /// No of people, order, and starting person are determined using LoadParams
  /// </summary>
  /// <param name="loadParams">Paging and sorting parameters</param>
  /// <returns></returns>
  [HttpGet(Name = "GetPeople")]
  public async Task<List<PersonDTO>> GetAll([FromQuery] LoadParams loadParams) 
  {
    var query = ctx.People.ProjectToDTO();

    SieveModel sieveModel = new SieveModel()
    {
      Filters = loadParams.Filters,
      Sorts = loadParams.GetSort(nameof(PersonDTO.FirstName)),
      PageSize = loadParams.PageSize,
      Page = loadParams.Page
    };

    var filteredQuery = sieveProcessor.Apply(sieveModel, query);

    var list = await filteredQuery.ToListAsync(HttpContext.RequestAborted);
    return list;
  }  

  /// <summary>
  /// Get person by id 
  /// </summary>
  /// <param name="id">person id</param>
  /// <returns>person the internally has set id to the id from the route</returns>
  [HttpGet("{id}", Name = "GetPersonById")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<PersonDTO>> Get(int id)
  {
    var person = await ctx.People
                          .Where(m => m.Id == id)
                          .ProjectToDTO()
                          .FirstOrDefaultAsync(HttpContext.RequestAborted);
    if (person == null)
    {      
      return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No person with id = {id}");
    }
    else
    {
      return person;
    }
  }


  /// <summary>
  /// Delete person base on its primary key
  /// </summary>
  /// <param name="id">Value of the primary key</param>
  /// <returns></returns>
  /// <response code="204">if the person is deleted</response>
  /// <response code="404">if person does not exists</response>      
  [HttpDelete("{id}", Name = "DeletePerson")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Delete(int id)
  {
    var person = await ctx.People.FindAsync(id, HttpContext.RequestAborted);
    if (person == null)
    {
      //return NotFound();
      return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
    }
    else
    {
      ctx.Remove(person);
      await ctx.SaveChangesAsync(HttpContext.RequestAborted);
      return NoContent();
    };     
  }

  /// <summary>
  /// Updates person
  /// </summary>
  /// <param name="id">person identifier (i.e., primary key)</param>
  /// <param name="model">Person data. id from route, and id from the model must match</param>
  /// <param name="validator"></param>
  /// <returns></returns>
  [HttpPut("{id}", Name = "UpdatePerson")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Update(int id, PersonDTO model, [FromServices] IValidator<PersonDTO> validator)
  {
    if (model.Id != id) //models are automatically validation because of [ApiController]
    {
      return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.Id}");
    }
    return await DoUpdate(id, model, validator);
  }

  /// <summary>
  /// Partially update the item
  /// </summary>
  /// <param name="id"></param>
  /// <param name="delta">RFC 6902 formatted json</param>
  /// <param name="validator"></param>    
  /// <returns></returns>
  /// <response code="204">if the update was successful</response>
  /// <response code="404">if there is no item with sent id, or if a user does not have a permission to update the item</response>
  /// <response code="400">If the patched model is not valid</response>
  [HttpPatch("{id}")]
  [Consumes("application/json-patch+json")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public virtual async Task<IActionResult> UpdatePartially(int id, JsonPatchDocument<PersonDTO> delta, [FromServices] IValidator<PersonDTO> validator)
  {
    var personDto = await ctx.People
                          .Where(m => m.Id == id)
                          .ProjectToDTO()
                          .FirstOrDefaultAsync(HttpContext.RequestAborted);
    if (personDto == null)
    {
      return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No person with id = {id}");
    }

    string? problem = null;
    delta.ApplyTo(personDto, patchError =>
    {
      problem = $"{patchError.Operation} causing error: {patchError.ErrorMessage}";
    });
    if (problem != null)
    {
      return Problem(detail: problem, statusCode: StatusCodes.Status400BadRequest);
    }

    if (personDto.Id != id) //ensures that id has not been changed
    {
      return Problem(detail: $"Id mismatch after patching {id} <> {personDto.Id}", statusCode: StatusCodes.Status400BadRequest);
    }

    return await DoUpdate(id, personDto, validator);
  }

  /// <summary>
  /// Common part of Update and UpdatePartially: loads the person, validates the model,
  /// copies the model into the entity and saves changes
  /// </summary>
  /// <param name="id">person identifier (already checked against model.Id)</param>
  /// <param name="model">complete person data (sent or patched)</param>
  /// <param name="validator"></param>
  /// <returns>204, 404, or 400 with validation errors</returns>
  private async Task<IActionResult> DoUpdate(int id, PersonDTO model, IValidator<PersonDTO> validator)
  {
    var person = await ctx.People.FindAsync(id, HttpContext.RequestAborted);
    if (person == null)
    {
      return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
    }

    var validationResult = await validator.ValidateAsync(model, HttpContext.RequestAborted);
    if (!validationResult.IsValid)
    {
      return ValidationProblem(new ValidationProblemDetails(validationResult.ToDictionary()));
    }

    model.MapToPerson(person);

    await ctx.SaveChangesAsync(HttpContext.RequestAborted);
    return NoContent();
  }

  /// <summary>
  /// Adds new person
  /// </summary>
  /// <param name="model">Person data</param>
  /// <param name="validator"></param>
  /// <returns></returns>
  [HttpPost(Name = "AddPerson")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> Create(PersonDTO model, [FromServices] IValidator<PersonDTO> validator)
  {
    var validationResult = await validator.ValidateAsync(model, HttpContext.RequestAborted);
    if (!validationResult.IsValid)
    {
      return ValidationProblem(
        new ValidationProblemDetails(validationResult.ToDictionary()));
    }

    Person person = new();
    model.MapToPerson(person);
    ctx.Add(person);
    await ctx.SaveChangesAsync(HttpContext.RequestAborted); 

    var addedItem = await Get(person.Id);

    return CreatedAtAction(nameof(Get), new { id = person.Id }, addedItem.Value);
  }
}
