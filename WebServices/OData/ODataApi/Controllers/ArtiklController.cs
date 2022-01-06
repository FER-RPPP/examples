using EFModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ODataApi.Controllers
{
  public class ArtiklController : ODataController
  {
    private readonly FirmaContext ctx;
    private readonly ILogger logger;
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="ctx">FirmaContext za pristup bazi podataka</param>
    /// <param name="logger">Logger za evidentiranje pogrešaka</param>
    public ArtiklController(FirmaContext ctx, ILogger<ArtiklController> logger)
    {
      this.logger = logger;
      this.ctx = ctx;
    }

    // GET: odata/Artikl
    /// <summary>
    /// Postupak za dohvat svih artikala prema kriterijima unutar OData zahtjeva. 
    /// </summary>
    /// <returns>Popis odabranih dokumenata. Vraća IQueryable i omogućava upite koristeći OData</returns>
    [EnableQuery(PageSize = 50)]
    public IQueryable<Artikl> Get()
    {
      logger.LogTrace($"Get {Request.QueryString.Value}");
      var query = ctx.Artikl.AsNoTracking();
      return query;
    }

    // GET: odata/Artikl(key)
    /// <summary>
    /// Postupak za dohvat nekog artikla. 
    /// </summary>
    /// <returns>Podatak o artiklu</returns>
    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
      logger.LogTrace($"Get + key = {key} {Request.QueryString.Value}");
      var query = ctx.Artikl
                      .AsNoTracking()
                      .Where(d => d.SifArtikla == key);                              
      if (await query.AnyAsync())
      {
        return Ok(query);        
      }
      else
      {
        return NotFound("Traženi artikl ne postoji");
      }
    }
  }
}