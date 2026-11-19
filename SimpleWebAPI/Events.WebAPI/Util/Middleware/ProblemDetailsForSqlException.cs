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
        Detail = "A record with the same data already exists.",
        Status = StatusCodes.Status409Conflict
      },
      // deleting a record that other records still refer to: valid request, but in conflict with the current state
      PostgresErrorCodes.ForeignKeyViolation when HttpMethods.IsDelete(context.HttpContext.Request.Method) => new ProblemDetails
      {
        Title = "Record is in use",
        Detail = $"The record cannot be deleted because other records refer to it. {postgresException.MessageText}",
        Status = StatusCodes.Status409Conflict
      },
      // insert or update referring to a non-existing record: invalid request
      PostgresErrorCodes.ForeignKeyViolation => new ProblemDetails
      {
        Title = "Missing or invalid related data",
        Detail = $"The operation is not allowed because it violates a foreign key constraint. {postgresException.MessageText}",
        Status = StatusCodes.Status400BadRequest
      },
      _ => new ProblemDetails
      {
        Title = "Database error",
        Detail = $"An error occurred while saving data to the database. {postgresException.MessageText}",
        Status = StatusCodes.Status500InternalServerError
      }
    };

    logger.LogDebug("Database exception: {message}", context.Exception.CompleteExceptionMessage());
    context.ExceptionHandled = true;
    context.Result = new ObjectResult(problemDetails)
    {
      ContentTypes = { "application/problem+json" },
      StatusCode = problemDetails.Status
    };
  }
}
