using EFModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApi.Models;
using WebServices.Util.ExceptionFilters;

namespace WebApi.Controllers
{
  /// <summary>
  /// Web API servis za rad s mjestima
  /// </summary>
  [ApiController]
  [Route("[controller]")]
  [TypeFilter(typeof(ProblemDetailsForSqlException))]
  public class MjestoController : ControllerBase, ICustomController<int, MjestoViewModel>
  {
    private readonly FirmaContext ctx;
    private static Dictionary<string, Expression<Func<Mjesto, object>>> orderSelectors = new Dictionary<string, Expression<Func<Mjesto, object>>>
    {
      [nameof(MjestoViewModel.IdMjesta).ToLower()] = m => m.IdMjesta,
      [nameof(MjestoViewModel.NazivDrzave).ToLower()] = m => m.OznDrzaveNavigation.NazDrzave,
      [nameof(MjestoViewModel.NazivMjesta).ToLower()] = m => m.NazMjesta,
      [nameof(MjestoViewModel.OznDrzave).ToLower()] = m => m.OznDrzave,
      [nameof(MjestoViewModel.PostBrojMjesta).ToLower()] = m => m.PostBrMjesta,
      [nameof(MjestoViewModel.PostNazivMjesta).ToLower()] = m => m.PostNazMjesta
    };

    public MjestoController(FirmaContext ctx) 
    {
      this.ctx = ctx;
    }


    /// <summary>
    /// Vraća broj svih mjesta filtriran prema nazivu mjesta 
    /// </summary>
    /// <param name="filter">Opcionalni filter za naziv mjesta</param>
    /// <returns></returns>
    [HttpGet("count", Name = "BrojMjesta")]
    public async Task<int> Count([FromQuery] string filter)
    {
      var query = ctx.Mjesto.AsQueryable();
      if (!string.IsNullOrWhiteSpace(filter))
      {
        query = query.Where(m => m.NazMjesta.Contains(filter));
      }
      int count = await query.CountAsync();
      return count;
    }

    /// <summary>
    /// Dohvat mjesta (opcionalno filtrirano po nazivu mjesta).
    /// Broj mjesta, poredak, početna pozicija određeni s loadParams.
    /// </summary>
    /// <param name="loadParams">Postavke za straničenje i filter</param>
    /// <returns></returns>
    [HttpGet(Name = "DohvatiMjesta")]
    public async Task<List<MjestoViewModel>> GetAll([FromQuery] LoadParams loadParams) 
    {
      var query = ctx.Mjesto.AsQueryable();

      if (!string.IsNullOrWhiteSpace(loadParams.Filter))
      {
        query = query.Where(m => m.NazMjesta.Contains(loadParams.Filter));
      }

      if (loadParams.SortColumn != null)
      {
        if (orderSelectors.TryGetValue(loadParams.SortColumn.ToLower(), out var expr))
        {
          query = loadParams.Descending ? query.OrderByDescending(expr) : query.OrderBy(expr);
        }
      }

      var list = await query.Select(m => new MjestoViewModel
                            {
                              IdMjesta = m.IdMjesta,
                              NazivDrzave = m.OznDrzaveNavigation.NazDrzave,
                              NazivMjesta = m.NazMjesta,
                              OznDrzave = m.OznDrzave,
                              PostBrojMjesta = m.PostBrMjesta,
                              PostNazivMjesta = m.PostNazMjesta
                            })
                            .Skip(loadParams.StartIndex)
                            .Take(loadParams.Rows)
                            .ToListAsync();
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
    public async Task<ActionResult<MjestoViewModel>> Get(int id)
    {   
      var mjesto = await ctx.Mjesto                            
                            .Where(m => m.IdMjesta == id)
                            .Select(m => new MjestoViewModel
                            {
                              IdMjesta = m.IdMjesta,
                              NazivDrzave = m.OznDrzaveNavigation.NazDrzave,
                              NazivMjesta = m.NazMjesta,
                              OznDrzave = m.OznDrzave,
                              PostBrojMjesta = m.PostBrMjesta,
                              PostNazivMjesta = m.PostNazMjesta
                            })
                            .FirstOrDefaultAsync();
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
      var mjesto = await ctx.Mjesto.FindAsync(id);
      if (mjesto == null)
      {
        return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
      }
      else
      {
        ctx.Remove(mjesto);
        await ctx.SaveChangesAsync();
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
    public async Task<IActionResult> Update(int id, MjestoViewModel model)
    {
      if (model.IdMjesta != id) //ModelState.IsValid i model != null provjera se automatski zbog [ApiController]
      {
        return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.IdMjesta}");
      }
      else
      {
        var mjesto = await ctx.Mjesto.FindAsync(id);
        if (mjesto == null)
        { 
          return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
        }

        mjesto.NazMjesta = model.NazivMjesta;
        mjesto.OznDrzave = model.OznDrzave;
        mjesto.PostBrMjesta = model.PostBrojMjesta;
        mjesto.PostNazMjesta = model.PostNazivMjesta;

        await ctx.SaveChangesAsync();
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
    public async Task<IActionResult> Create(MjestoViewModel model)
    {
      Mjesto mjesto = new Mjesto
      {
        NazMjesta = model.NazivMjesta,
        PostBrMjesta = model.PostBrojMjesta,
        OznDrzave = model.OznDrzave,
        PostNazMjesta = model.PostNazivMjesta
      };
      ctx.Add(mjesto);
      await ctx.SaveChangesAsync();

      var addedItem = await Get(mjesto.IdMjesta);

      return CreatedAtAction(nameof(Get), new { id = mjesto.IdMjesta }, addedItem.Value);
    }
  }
}
