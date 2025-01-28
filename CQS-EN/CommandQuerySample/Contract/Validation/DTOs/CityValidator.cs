using Contract.DTOs;
using FluentValidation;


namespace Contract.Validation.DTOs
{
  public class CityValidator : AbstractValidator<City>
  {
    public CityValidator()
    {
      RuleFor(m => m.CountryCode).NotEmpty();

      RuleFor(m => m.CityName).NotEmpty();

      RuleFor(m => m.PostalCode)
        .InclusiveBetween(10, 60000);
    }
  }
}
