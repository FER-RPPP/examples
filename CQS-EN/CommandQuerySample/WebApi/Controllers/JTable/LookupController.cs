using Contract.Queries;
using Contract.QueryHandlers;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models.JTable;

namespace WebApi.Controllers.JTable;

/// <summary>
/// Lookup controller suitable for jTable
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class LookupController : ControllerBase
{
  private readonly ICountriesLookupQueryHandler countriesLookupQueryHandler;

  public LookupController(ICountriesLookupQueryHandler countriesLookupQueryHandler) 
  {
    this.countriesLookupQueryHandler = countriesLookupQueryHandler;
  }

  [HttpGet]
  [HttpPost]
  public async Task<OptionsResult> Countries()
  {
    var query = new CountriesLookupQuery();
    var countries = await countriesLookupQueryHandler.Handle(query);
    var options = countries.Select(d => new TextValue
                           {
                             DisplayText = d.Text,
                             Value = d.Value
                           })
                           .ToList();
    return new OptionsResult(options);
  }
}
