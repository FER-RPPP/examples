using FluentValidation;
using MVC_EN.Models;

namespace MVC_EN.ModelsValidation;

public class CountryValidator : AbstractValidator<Country>
{
  public CountryValidator()
  {
    RuleFor(d => d.CountryCode)
      .NotEmpty()      
      .MaximumLength(2);

    RuleFor(d => d.CountryName).NotEmpty();

    RuleFor(d => d.CountryIso3).MaximumLength(3);        
  }
}
