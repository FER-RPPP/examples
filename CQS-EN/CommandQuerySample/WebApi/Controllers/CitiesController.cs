﻿using AutoMapper;
using CommandQueryCore;
using Contract.Commands;
using Contract.DTOs;
using Contract.Queries;
using Contract.QueryHandlers;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Util.ExceptionFilters;
using WebServices.Util.ExceptionFilters;

namespace WebApi.Controllers
{
  /// <summary>
  /// Web API service for cities CRUD operations
  /// </summary>
  [ApiController]
  [Route("[controller]")]
  [TypeFilter(typeof(ProblemDetailsForSqlException))]
  [TypeFilter(typeof(BadRequestOnRuleValidationException))]
  public class CitiesController : ControllerBase, ICustomController<int, City>
  {
    private readonly IMapper mapper;
    private readonly ICitiesCountQueryHandler citiesCountQueryHandler;
    private readonly ICitiesQueryHandler citiesQueryHandler;
    private readonly ICityQueryHandler cityQueryHandler;
    private readonly ICommandHandler<AddCity, int> addCityCommandHandler;
    private readonly ICommandHandler<UpdateCity> updateCityCommandHandler;
    private readonly ICommandHandler<DeleteCity> deleteCityCommandHandler;

    //Alternative: use [FromServices] for each action
    public CitiesController(
      IMapper mapper,
      ICitiesCountQueryHandler citiesCountQueryHandler,
      ICitiesQueryHandler citiesQueryHandler,
      ICityQueryHandler cityQueryHandler,
      ICommandHandler<AddCity, int> addCityCommandHandler,
      ICommandHandler<UpdateCity> updateCityCommandHandler,
      ICommandHandler<DeleteCity> deleteCityCommandHandler
      )
    {
      this.mapper = mapper;
      this.citiesCountQueryHandler = citiesCountQueryHandler;
      this.citiesQueryHandler = citiesQueryHandler;
      this.cityQueryHandler = cityQueryHandler;
      this.addCityCommandHandler = addCityCommandHandler;
      this.updateCityCommandHandler = updateCityCommandHandler;
      this.deleteCityCommandHandler = deleteCityCommandHandler;
    }


    /// <summary>
    /// returns number of cities satisfying filter (by city name)
    /// </summary>
    /// <param name="filter">Optional filter for city name</param>
    /// <returns></returns>
    [HttpGet("count", Name = "CitiesNumber")]
    public async Task<int> Count([FromQuery] string filter)
    {
      var query = new CitiesCountQuery()
      {
        SearchText = filter
      };
      int count = await citiesCountQueryHandler.Handle(query);
      return count;
    }


    /// <summary>
    /// Get all cities (optionally filtered by city name)
    /// No of cities, order, and starting city are determined using LoadParams
    /// </summary>
    /// <param name="loadParams">Paging and sortin parameters</param>
    /// <returns></returns>
    [HttpGet(Name = "GetCities")]
    public async Task<List<City>> GetAll([FromQuery] LoadParams loadParams)
    {
      var query = new CitiesQuery
      {
        Count = loadParams.Rows,
        From = loadParams.StartIndex,
        SearchText = loadParams.Filter
      };
      if (loadParams.SortColumn != null)
      {
        query.Sort = new SortInfo();
        query.Sort.ColumnOrder.Add(new KeyValuePair<string, SortInfo.Order>(loadParams.SortColumn, loadParams.Descending ? SortInfo.Order.DESCENDING : SortInfo.Order.ASCENDING));
      }
      var list = await citiesQueryHandler.Handle(query);
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
    public async Task<ActionResult<City>> Get(int id)
    {
      var query = new CityQuery
      {
        Id = id
      };
      var city = await cityQueryHandler.Handle(query);
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
      var city = await cityQueryHandler.Handle(new CityQuery
      {
        Id = id
      });

      if (city == null)
      {
        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
      }
      else
      {
        await deleteCityCommandHandler.Handle(new DeleteCity(id));
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
    public async Task<IActionResult> Update(int id, City model)
    {
      if (model.CityId != id) //ModelState.IsValid i model != null provjera se automatski zbog [ApiController]
      {
        return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.CityId}");
      }
      else
      {
        var city = await cityQueryHandler.Handle(new CityQuery
        {
          Id = id
        });
        if (city == null)
        {
          return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
        }

        UpdateCity command = mapper.Map<UpdateCity>(model);
        await updateCityCommandHandler.Handle(command);

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
    public async Task<IActionResult> Create(City model)
    {
      AddCity command = mapper.Map<AddCity>(model);
      int id = await addCityCommandHandler.Handle(command);

      var addedItem = await cityQueryHandler.Handle(new CityQuery
      {
        Id = id
      });

      return CreatedAtAction(nameof(Get), new { id }, addedItem);
    }
  }
}