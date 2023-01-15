using EFModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models.JTable;

namespace WebApi.Controllers.JTable
{
  /// <summary>
  /// Lookup controller suitable for jTable
  /// </summary>
  [ApiController]
  [Route("[controller]/[action]")]
  public class LookupController : ControllerBase
  {
    private readonly FirmContext ctx;

    public LookupController(FirmContext ctx) 
    {
      this.ctx = ctx;
    }

    [HttpGet]
    [HttpPost]
    public async Task<OptionsResult> Countries()
    {
      var options = await ctx.Countries
                             .OrderBy(d => d.CountryName)
                             .Select(d => new TextValue
                             {
                               DisplayText = d.CountryName,
                               Value = d.CountryCode
                             })
                             .ToListAsync();
      return new OptionsResult(options);
    }
  }
}
