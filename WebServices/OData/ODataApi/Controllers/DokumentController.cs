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
  public class DokumentController : ODataController
  {
    private readonly FirmaContext ctx;
    private readonly ILogger logger;
    /// <summary>
    /// Konstruktor
    /// </summary>
    /// <param name="ctx">FirmaContext za pristup bazi podataka</param>
    /// <param name="logger">Logger za evidentiranje pogrešaka</param>
    public DokumentController(FirmaContext ctx, ILogger<DokumentController> logger)
    {
      this.logger = logger;
      this.ctx = ctx;
    }

    // GET: odata/Dokument
    /// <summary>
    /// Postupak za dohvat svih dokument prema kriterijima unutar OData zahtjeva. 
    /// </summary>
    /// <returns>Popis odabranih dokumenata. Vraća IQueryable i omogućava upite koristeći OData</returns>
    [EnableQuery(PageSize = 50)]
    public IQueryable<Dokument> Get()
    {
      logger.LogTrace($"Get {Request.QueryString.Value}");
      var query = ctx.Dokument.AsNoTracking();
      return query;
    }

    // GET: odata/Dokument(key)
    /// <summary>
    /// Postupak za dohvat nekog dokumenta. 
    /// </summary>
    /// <returns>Podatak o dokumentu</returns>
    [EnableQuery]
    public async Task<IActionResult> Get(int key)
    {
      logger.LogTrace($"Get + key = {key} {Request.QueryString.Value}");
      var query = ctx.Dokument
                      .AsNoTracking()
                      .Where(d => d.IdDokumenta == key);                              
      if (await query.AnyAsync())
      {
        return Ok(query);        
      }
      else
      {
        return NotFound("Traženi dokument ne postoji");
      }
    }
  }
}