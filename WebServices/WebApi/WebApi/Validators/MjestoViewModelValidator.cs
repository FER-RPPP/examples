﻿using FluentValidation;
using WebApi.Models;

namespace WebApi.Validators
{
  public class MjestoViewModelValidator : AbstractValidator<MjestoViewModel>
  {
    public MjestoViewModelValidator()
    {
      RuleFor(m => m.OznDrzave)
        .NotEmpty().WithMessage("Potrebno je unijeti oznaku države");           

      RuleFor(m => m.NazivMjesta)        
        .NotEmpty().WithMessage("Potrebno je unijeti naziv mjesta");

      RuleFor(m => m.PostBrojMjesta)
        .NotEmpty().WithMessage("Potrebno je unijeti poštanski broj mjesta (10-60000)")
        .GreaterThanOrEqualTo(10).WithMessage("Dozvoljeni raspon: 10-60000")
        .LessThanOrEqualTo(60000).WithMessage("Dozvoljeni raspon: 10-60000");
    }
  }
}
