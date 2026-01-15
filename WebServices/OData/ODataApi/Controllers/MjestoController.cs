using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataApi.Contract;
using System.Text;
using System.Text.Json;

namespace ODataApi.Controllers;
  
public class MjestoController : ODataController
{
  private readonly EFModel.FirmaContext ctx;
  private readonly ILogger logger;
  /// <summary>
  /// Konstruktor
  /// </summary>
  /// <param name="ctx">FirmaContext za pristup bazi podataka</param>
  /// <param name="logger">Logger za evidentiranje pogrešaka</param>
  public MjestoController(EFModel.FirmaContext ctx, ILogger<MjestoController> logger)
  {
    this.logger = logger;
    this.ctx = ctx;
  }

  // GET: odata/Mjesto
  /// <summary>
  /// Postupak za dohvat svih mjesta (vraća maksimalno 50 mjesta u jednom upitu). 
  /// </summary>
  /// <returns>Popis svih mjesta. Vraća IQueryable i omogućava upite koristeći OData</returns>    
  [EnableQuery(PageSize = 50)]
  public IQueryable<Mjesto> Get()
  {
    logger.LogTrace("Get: " + Request.QueryString.Value);
    var query = ctx.Mjesto
                   .Select(m => new Mjesto
                   {
                     IdMjesta = m.IdMjesta,
                     NazivMjesta = m.NazMjesta,
                     PostBrojMjesta = m.PostBrMjesta,
                     PostNazivMjesta = m.PostNazMjesta,
                     OznDrzave = m.OznDrzave,
                     NazivDrzave = m.OznDrzaveNavigation.NazDrzave
                   });
    return query;
  }

  // GET: odata/Mjesto(key)
  /// <summary>
  /// Postupak za dohvat podataka nekog mjesta. 
  /// </summary>
  /// <returns>Podatak o mjestu</returns>
  public async Task<IActionResult> Get(int key)
  {
    logger.LogTrace($"Get + key = {key} {Request.QueryString.Value}");
    var mjesto = await ctx.Mjesto
                          .Where(m => m.IdMjesta == key)
                          .Select(m => new Mjesto
                          {
                            IdMjesta = m.IdMjesta,
                            NazivMjesta = m.NazMjesta,
                            PostBrojMjesta = m.PostBrMjesta,
                            PostNazivMjesta = m.PostNazMjesta,
                            OznDrzave = m.OznDrzave,
                            NazivDrzave = m.OznDrzaveNavigation.NazDrzave
                          })
                          .FirstOrDefaultAsync();
    if (mjesto == null)
    {
      return NotFound($"Mjesto s ključem {key} ne postoji");
    }
    else
    {
      return Ok(mjesto);
    }
  }

  // POST /odata/Mjesto
  [HttpPost]
  public async Task<IActionResult> Post([FromBody] Mjesto model)
  {
    using (MemoryStream stream = new MemoryStream())
    {
      await Request.Body.CopyToAsync(stream);
      logger.LogTrace(Encoding.Default.GetString(stream.ToArray()));
    }
    logger.LogTrace(JsonSerializer.Serialize(model));

    if (model != null && ModelState.IsValid)
    {
      var mjesto = new EFModel.Mjesto
      {
        NazMjesta = model.NazivMjesta,
        PostBrMjesta = model.PostBrojMjesta,
        PostNazMjesta = model.PostNazivMjesta,
        OznDrzave = model.OznDrzave
      };
     
      ctx.Add(mjesto);
      await ctx.SaveChangesAsync();
      model.IdMjesta = mjesto.IdMjesta;

      return Created(model);
    }
    else
    {
      return BadRequest(ModelState);
    }
  }

  // PUT /odata/Mjesto(key)
  [HttpPut]
  public async Task<IActionResult> Put(int key, [FromBody] Mjesto model)
  {
    using (MemoryStream stream = new MemoryStream())
    {
      await Request.Body.CopyToAsync(stream);
      logger.LogTrace(Encoding.Default.GetString(stream.ToArray()));
    }
    logger.LogTrace(JsonSerializer.Serialize(model));

    if (model == null || model.IdMjesta != key || !ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }
    else
    {
      var mjesto = await ctx.Mjesto.FindAsync(key);
      if (mjesto == null)
      {
        return NotFound("Traženo mjesto ne postoji");
      }
      else
      {
        mjesto.NazMjesta = model.NazivMjesta;
        mjesto.PostBrMjesta = model.PostBrojMjesta;
        mjesto.PostNazMjesta = model.PostNazivMjesta;
        mjesto.OznDrzave = model.OznDrzave;

        await ctx.SaveChangesAsync();
        return Updated(model);
      };
    }
  }

  // PATCH /odata/Mjesto(key)
  [HttpPatch]
  public async Task<IActionResult> Patch(int key, [FromBody] Delta<Mjesto> model)
  {
    using (MemoryStream stream = new MemoryStream())
    {
      await Request.Body.CopyToAsync(stream);
      logger.LogTrace(Encoding.Default.GetString(stream.ToArray()));
    }
    logger.LogTrace(JsonSerializer.Serialize(model));

    if (model == null || !ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }
    else
    {
      var mjesto = await ctx.Mjesto.FindAsync(key);
      if (mjesto == null)
      {
        return NotFound("Traženo mjesto ne postoji");
      }
      else
      {                    
        var viewmodel = new Mjesto
        {
          IdMjesta = mjesto.IdMjesta,
          NazivMjesta = mjesto.NazMjesta,
          PostBrojMjesta = mjesto.PostBrMjesta,
          PostNazivMjesta = mjesto.PostNazMjesta,
          OznDrzave = mjesto.OznDrzave,
        };

        model.Patch(viewmodel);

        mjesto.NazMjesta = viewmodel.NazivMjesta;
        mjesto.PostBrMjesta = viewmodel.PostBrojMjesta;
        mjesto.PostNazMjesta = viewmodel.PostNazivMjesta;
        mjesto.OznDrzave = viewmodel.OznDrzave;

        await ctx.SaveChangesAsync();
        return Updated(viewmodel);
      };
    }
  }

  // DELETE /odata/Mjesto(key)   
  [HttpDelete]
  public async Task<IActionResult> Delete(int key)
  {
    var mjesto = await ctx.Mjesto.FindAsync(key);
    if (mjesto == null)
    {
      return NotFound("Traženo mjesto ne postoji");
    }
    else
    {
      ctx.Remove(mjesto);
      await ctx.SaveChangesAsync();
      return NoContent();
    }
  }
}
