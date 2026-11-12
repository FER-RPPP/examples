using Events.WebAPI.Util.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Events.WebAPI.Util.Middleware;

public class ProblemDetailsForException : ExceptionFilterAttribute
{
  private readonly ILogger<ProblemDetailsForException> logger;

  public ProblemDetailsForException(ILogger<ProblemDetailsForException> logger)
  {
    this.logger = logger;
  }

  public override void OnException(ExceptionContext context)
  {
    string exceptionMessage = context.Exception.CompleteExceptionMessage();
    logger.LogError("Error 500: {0}", exceptionMessage); //TO DO: Log data from context.ActionDescriptor?      
    logger.LogError(context.Exception.StackTrace);
    context.ExceptionHandled = true;
    var problemDetails = new ProblemDetails
    {
      Type = "https://httpstatuses.io/500",
      Detail = exceptionMessage,
      Title = "Internal server error",
      Instance = context.HttpContext.TraceIdentifier
    };
    context.Result = new ObjectResult(problemDetails)
    {
      ContentTypes = { "application/problem+json" },
      StatusCode = StatusCodes.Status500InternalServerError
    };
  }
}
