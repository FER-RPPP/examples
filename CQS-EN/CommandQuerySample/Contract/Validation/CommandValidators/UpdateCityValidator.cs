using CommandQueryCore;
using Contract.Commands;
using Contract.DTOs;
using Contract.Queries;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Contract.Validation.CommandValidators
{
  public class UpdateCityValidator : AbstractValidator<UpdateCity>
  {
    private readonly IQueryHandler<SearchCitiesQuery, IEnumerable<City>> searchCitiesQueryHandler;

    public UpdateCityValidator(IQueryHandler<SearchCitiesQuery, IEnumerable<City>> searchCitiesQueryHandler)
    {
      this.searchCitiesQueryHandler = searchCitiesQueryHandler;

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
      var cities = await searchCitiesQueryHandler.Handle(searchQuery);
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
}
