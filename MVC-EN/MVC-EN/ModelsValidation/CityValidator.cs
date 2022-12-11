using FluentValidation;
using MVC_EN.Models;

namespace MVC_EN.ModelsValidation;

public class CityValidator : AbstractValidator<City>
{
  public CityValidator()
  {
    RuleFor(m => m.CountryCode).NotEmpty();

    RuleFor(m => m.CityName).NotEmpty();

    RuleFor(m => m.PostalCode).InclusiveBetween(10, 60000);
  }
}
