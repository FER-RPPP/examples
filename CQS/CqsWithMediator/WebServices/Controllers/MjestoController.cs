﻿using AutoMapper;
using Contract.Commands;
using Contract.DTOs;
using Contract.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebServices.Util.ExceptionFilters;
using WebServices.ViewModels;

namespace WebServices.Controllers
{
  /// <summary>
  /// Web API servis za rad s mjestima
  /// </summary>
  [ApiController]
  [Route("[controller]")]
  [TypeFilter(typeof(ProblemDetailsForSqlException))]
  [TypeFilter(typeof(BadRequestOnRuleValidationException))]
  public class MjestoController : ControllerBase, ICustomController<int, Mjesto>
  {
    private readonly IMediator mediator;
    private readonly IMapper mapper;    
    
    public MjestoController(IMediator mediator, IMapper mapper)
    {
      this.mediator = mediator;
      this.mapper = mapper;     
    }


    /// <summary>
    /// Vraća broj svih mjesta filtriran prema nazivu mjesta 
    /// </summary>
    /// <param name="filter">Opcionalni filter za naziv mjesta</param>
    /// <returns></returns>
    [HttpGet("count", Name = "BrojMjesta")]
    public async Task<int> Count([FromQuery] string filter)
    {
      var query = new MjestoCountQuery()
      {
        SearchText = filter
      };
      int count = await mediator.Send(query);
      return count;
    }

    /// <summary>
    /// Dohvat mjesta (opcionalno filtrirano po nazivu mjesta).
    /// Broj mjesta, poredak, početna pozicija određeni s loadParams.
    /// </summary>
    /// <param name="loadParams">Postavke za straničenje i filter</param>
    /// <returns></returns>
    [HttpGet(Name = "DohvatiMjesta")]
    public async Task<IEnumerable<Mjesto>> GetAll([FromQuery] LoadParams loadParams) 
    {
      var query = new MjestaQuery
      {
        Count = loadParams.Rows,
        From = loadParams.StartIndex,
        SearchText = loadParams.Filter        
      };
      query.Sort = new SortInfo();
      query.Sort.ColumnOrder.Add(new KeyValuePair<string, SortInfo.Order>(loadParams.SortColumn, loadParams.Descending ? SortInfo.Order.DESCENDING : SortInfo.Order.ASCENDING));
      var list = await mediator.Send(query);
      return list;      
    }  

    /// <summary>
    /// Vraća grad čiji je IdMjesta jednak vrijednosti parametra id
    /// </summary>
    /// <param name="id">IdMjesta</param>
    /// <returns></returns>
    [HttpGet("{id}", Name = "DohvatiMjesto")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Mjesto>> Get(int id)
    {
      var query = new MjestoQuery
      {
        Id = id
      };
      var mjesto = await mediator.Send(query);
      if (mjesto == null)
      {      
        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for id = {id}");
      }
      else
      {
        return mjesto;
      }
    }


    /// <summary>
    /// Brisanje mjesta određenog s id
    /// </summary>
    /// <param name="id">Vrijednost primarnog ključa (Id mjesta)</param>
    /// <returns></returns>
    /// <response code="204">Ako je mjesto uspješno obrisano</response>
    /// <response code="404">Ako mjesto s poslanim id-om ne postoji</response>      
    [HttpDelete("{id}", Name = "ObrisiMjesto")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
      var mjesto = await mediator.Send(new MjestoQuery
      {
        Id = id
      });
      
      if (mjesto == null)
      {
        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
      }
      else
      {
        await mediator.Send(new DeleteMjesto(id));
        return NoContent();
      };     
    }

    /// <summary>
    /// Ažurira mjesto
    /// </summary>
    /// <param name="id">parametar čija vrijednost jednoznačno identificira mjesto</param>
    /// <param name="model">Podaci o mjestu. IdMjesta mora se podudarati s parametrom id</param>
    /// <returns></returns>
    [HttpPut("{id}", Name = "AzurirajMjesto")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, Mjesto model)
    {     
      if (model.IdMjesta != id) //ModelState.IsValid i model != null provjera se automatski zbog [ApiController]
      {
        return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.IdMjesta}");
      }
      else
      {
        var mjesto = await mediator.Send(new MjestoQuery
        {
          Id = id
        });
        if (mjesto == null)
        { 
          return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
        }

        UpdateMjesto command = mapper.Map<UpdateMjesto>(model);
        await mediator.Send(command);

        return NoContent();
      }
    }

    /// <summary>
    /// Stvara novo mjesto opisom poslanim modelom
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost(Name = "DodajMjesto")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(Mjesto model)
    {
      AddMjesto command = mapper.Map<AddMjesto>(model);
      int id = await mediator.Send(command);

      var addedItem = await mediator.Send(new MjestoQuery
      {
        Id = id
      });

      return CreatedAtAction(nameof(Get), new { id  }, addedItem);
    }
  }
}
