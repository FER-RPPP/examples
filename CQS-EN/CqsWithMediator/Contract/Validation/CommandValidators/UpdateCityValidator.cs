using Contract.Commands;
using Contract.Queries;
using FluentValidation;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Contract.Validation.CommandValidators;

public class UpdateCityValidator : AbstractValidator<UpdateCity>
{
  private readonly IMediator mediator;

  public UpdateCityValidator(IMediator mediator)
  {
    this.mediator = mediator;

    RuleFor(m => m.CountryCode).NotEmpty();

    RuleFor(m => m.CityName).NotEmpty();

    RuleFor(m => m.PostalCode)
      .InclusiveBetween(10, 60000)
      .MustAsync(CheckUniqueIndex).WithMessage("Postal code must be unique in the country");

  }

  private async Task<bool> CheckUniqueIndex(UpdateCity command, int pbr, CancellationToken cancellationToken)
  {
    var searchQuery = new SearchCitiesQuery
    {
      CountryCode = command.CountryCode,
      PostalCode = pbr
    };
    var cities = await mediator.Send(searchQuery);
    if (cities.Count() > 0)
    {
      var city = cities.First();
      return city.CityId == command.CityId; 
    }
    else
    {
      return true;
    }
  }
}
