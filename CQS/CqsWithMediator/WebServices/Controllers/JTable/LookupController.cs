using Contract.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebServices.ViewModels.JTable;

namespace WebServices.Controllers.JTable
{
  /// <summary>
  /// Lookup controller prilagođen za jTable
  /// </summary>
  [ApiController]
  [Route("[controller]/[action]")]
  public class LookupController : ControllerBase
  {
    private readonly IMediator mediator;

    public LookupController(IMediator mediator) 
    {
      this.mediator = mediator;
    }

    [HttpGet]
    [HttpPost]
    public async Task<OptionsResult> Drzave()
    {
      var data = await mediator.Send(new DrzaveLookupQuery());
      var options = data.Select(d => new TextValue
                        {
                          DisplayText = d.Text,
                          Value = d.Value
                        })
                        .ToList();
      return new OptionsResult(options);
    }
  }
}
