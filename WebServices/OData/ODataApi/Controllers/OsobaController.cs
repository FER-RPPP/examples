using EFModel;
using Microsoft.Extensions.Logging;

namespace ODataApi.Controllers
{
  public class OsobaController : GenericController<Osoba>
  {  
    public OsobaController(FirmaContext ctx, ILogger<OsobaController> logger) : base(ctx, logger)
    {

    }
  }
}