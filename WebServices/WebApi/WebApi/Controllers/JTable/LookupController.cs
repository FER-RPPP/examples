using EFModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models.JTable;

namespace WebApi.Controllers.JTable
{
  /// <summary>
  /// Lookup controller prilagođen za jTable
  /// </summary>
  [ApiController]
  [Route("[controller]/[action]")]
  public class LookupController : ControllerBase
  {
    private readonly FirmaContext ctx;

    public LookupController(FirmaContext ctx) 
    {
      this.ctx = ctx;
    }

    [HttpGet]
    [HttpPost]
    public async Task<OptionsResult> Drzave()
    {
      var options = await ctx.Drzava
                             .OrderBy(d => d.NazDrzave)
                             .Select(d => new TextValue
                             {
                               DisplayText = d.NazDrzave,
                               Value = d.OznDrzave
                             })
                             .ToListAsync();
      return new OptionsResult(options);
    }
  }
}
