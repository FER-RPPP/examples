using EFModel;

namespace ODataApi.Controllers;

public class TvrtkaController : GenericController<Tvrtka>
{  
  public TvrtkaController(FirmaContext ctx, ILogger<TvrtkaController> logger) : base(ctx, logger)
  {

  }
}