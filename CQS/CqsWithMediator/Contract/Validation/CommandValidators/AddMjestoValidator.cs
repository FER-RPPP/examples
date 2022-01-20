﻿using Contract.Commands;
using Contract.DTOs;
using Contract.Queries;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Contract.Validation.CommandValidators
{
  public class AddMjestoValidator : AbstractValidator<AddMjesto>
  {
    private readonly IMediator mediator;

    public AddMjestoValidator(IMediator mediator)
    {
      this.mediator = mediator;

      RuleFor(m => m.OznDrzave)
        .NotEmpty().WithMessage("Potrebno je unijeti oznaku države");

      RuleFor(m => m.NazivMjesta)
        .NotEmpty().WithMessage("Potrebno je unijeti naziv mjesta");

      RuleFor(m => m.PostBrojMjesta)
        .NotEmpty().WithMessage("Potrebno je unijeti poštanski broj mjesta (10-60000)")
        .GreaterThanOrEqualTo(10).WithMessage("Dozvoljeni raspon: 10-60000")
        .LessThanOrEqualTo(60000).WithMessage("Dozvoljeni raspon: 10-60000")
        //Npr. neka u istoj državi ne mogu postojati 2 mjesta s istim poštanskim brojem
        .MustAsync(CheckUniqueIndex).WithMessage("Poštanski broj mora biti jedinstven na razini države");      
    }

    private async Task<bool> CheckUniqueIndex(AddMjesto command, int pbr, CancellationToken cancellationToken)
    {
      var searchQuery = new SearchMjestoQuery
      {
        OznDrzave = command.OznDrzave,
        PostanskiBroj = pbr
      };
      var mjesta = await mediator.Send(searchQuery);
      return mjesta.Count() == 0;
    }
  }
}
