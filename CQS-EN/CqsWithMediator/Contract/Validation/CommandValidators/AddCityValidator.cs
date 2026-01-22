using Contract.Commands;
using Contract.DTOs;
using Contract.Queries;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Contract.Validation.CommandValidators;

public class AddCityValidator : AbstractValidator<AddCity>
{
  private readonly IMediator mediator;

  public AddCityValidator(IMediator mediator)
  {
    this.mediator = mediator;

    RuleFor(m => m.CountryCode).NotEmpty();

    RuleFor(m => m.CityName).NotEmpty();

    RuleFor(m => m.PostalCode)
      .InclusiveBetween(10, 90000)
      //Post code should be unique in the country
      .MustAsync(CheckUniqueIndex).WithMessage("Postal code must be unique in the country");
  }

  private async Task<bool> CheckUniqueIndex(AddCity command, int pbr, CancellationToken cancellationToken)
  {
    var searchQuery = new SearchCitiesQuery
    {
      CountryCode = command.CountryCode,
      PostalCode = pbr
    };
    var cities = await mediator.Send(searchQuery);
    return cities.Count() == 0;
  }
}
