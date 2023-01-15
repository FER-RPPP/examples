using FluentValidation;
using WebApi.Models;

namespace WebApi.Validators
{
  public class CityViewModelValidator : AbstractValidator<CityViewModel>
  {
    public CityViewModelValidator()
    {
      RuleFor(m => m.CountryCode).NotEmpty();

      RuleFor(m => m.CityName).NotEmpty();

      RuleFor(m => m.PostalCode).InclusiveBetween(10, 60000);
    }
  }
}
