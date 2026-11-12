using Events.WebAPI.Util.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Events.WebAPI.Util.Middleware;

public class ProblemDetailsForSqlException : ExceptionFilterAttribute
{
  private readonly ILogger<ProblemDetailsForSqlException> logger;

  public ProblemDetailsForSqlException(ILogger<ProblemDetailsForSqlException> logger)
  {
    this.logger = logger;
  }

  public override void OnException(ExceptionContext context)
  {
    Exception? exception = context.Exception;
    PostgresException? postgresException = null;

    while (exception is not null)
    {
      if (exception is PostgresException currentPostgresException)
      {
        postgresException = currentPostgresException;
        break;
      }

      if (exception is DbUpdateException dbUpdateException && dbUpdateException.InnerException is not null)
      {
        exception = dbUpdateException.InnerException;
        continue;
      }

      exception = exception.InnerException;
    }

    if (postgresException is null)
    {
      base.OnException(context);
      return;
    }

    ProblemDetails problemDetails = postgresException.SqlState switch
    {
      PostgresErrorCodes.UniqueViolation => new ProblemDetails
      {
        Title = "Duplicate data",
        Detail = "A record with the same data already exists."
      },
      PostgresErrorCodes.ForeignKeyViolation => new ProblemDetails
      {
        Title = "Missing or invalidrelated data",
        Detail = $"The operation is not allowed because it violates a foreign key constraint. {postgresException.MessageText}"
      },
      _ => new ProblemDetails
      {
        Title = "Database error",
        Detail = $"An error occurred while saving data to the database. {postgresException.MessageText}"
      }
    };

    logger.LogDebug("Database exception: {message}", context.Exception.CompleteExceptionMessage());
    context.ExceptionHandled = true;
    context.Result = new ObjectResult(problemDetails)
    {
      ContentTypes = { "application/problem+json" },
      StatusCode = StatusCodes.Status500InternalServerError
    };
  }
}
