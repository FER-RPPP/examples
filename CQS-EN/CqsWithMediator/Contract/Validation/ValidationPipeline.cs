using MediatR;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Contract.Validation;

public class ValidationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
  private readonly IEnumerable<IValidator<TRequest>> validators;

  public ValidationPipeline(IEnumerable<IValidator<TRequest>> validators)
  {
    this.validators = validators;
  }

  public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
  {
    if (validators.Any())
    {
      var context = new ValidationContext<TRequest>(request);
      var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
      var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
      if (failures.Count != 0)
        throw new ValidationException(failures);
    }
    return await next();
  }
}
