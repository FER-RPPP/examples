using EFModel;

namespace ODataApi.Controllers;

public class OsobaController : GenericController<Osoba>
{  
  public OsobaController(FirmaContext ctx, ILogger<OsobaController> logger) : base(ctx, logger)
  {

  }
}