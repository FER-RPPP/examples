using Events.EF.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;


namespace Events.WebAPI.Models;

public class PersonDTOValidator : AbstractValidator<PersonDTO>
{
  private readonly EventsContext ctx;

  public PersonDTOValidator(EventsContext ctx)
  {
    this.ctx = ctx;
  
    RuleFor(a => a.FirstName).MaximumLength(100);
    RuleFor(a => a.LastName).MaximumLength(100);
    RuleFor(a => a.FirstNameTranscription).NotEmpty().MaximumLength(100);
    RuleFor(a => a.LastNameTranscription).NotEmpty().MaximumLength(100);
    RuleFor(a => a.AddressLine).MaximumLength(200);
    RuleFor(a => a.PostalCode).MaximumLength(20);
    RuleFor(a => a.City).MaximumLength(100);
    RuleFor(a => a.AddressCountry).MaximumLength(100);
    When(a => !string.IsNullOrWhiteSpace(a.Email), () =>
    {
      RuleFor(a => a.Email).MaximumLength(255).EmailAddress();
    });
    RuleFor(a => a.ContactPhone).MaximumLength(50);
    RuleFor(a => a.BirthDate).NotEmpty();
    RuleFor(a => a.DocumentNumber).NotEmpty().MaximumLength(50);
    RuleFor(a => a.CountryCode).NotEmpty().MaximumLength(3);

    RuleFor(a => a)
      .Must(a => !string.IsNullOrWhiteSpace(a.Email) || !string.IsNullOrWhiteSpace(a.ContactPhone))
      .WithMessage("Either Email or ContactPhone must be provided");

    RuleFor(a => a)
      .MustAsync(NoDuplicateDocuments) 
      .WithMessage("A person with the same document number and country code already exists.");
  }

  private async Task<bool> NoDuplicateDocuments(PersonDTO model, CancellationToken cancellationToken)
  {
    bool documentNumberExists = await ctx.People
                                         .AnyAsync(p => p.DocumentNumber == model.DocumentNumber 
                                                     && p.CountryCode == model.CountryCode
                                                     && p.Id != model.Id,
                                             cancellationToken);
    return !documentNumberExists;
  }
}
