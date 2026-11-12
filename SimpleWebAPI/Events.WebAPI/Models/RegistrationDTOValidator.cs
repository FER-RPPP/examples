using Events.EF.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;


namespace Events.WebAPI.Models;

public class RegistrationDTOValidator : AbstractValidator<RegistrationDTO>
{
  private readonly EventsContext ctx;

  public RegistrationDTOValidator(EventsContext ctx)
  {
    this.ctx = ctx;
    RuleFor(a => a.EventId).GreaterThan(0);
    RuleFor(a => a.PersonId).GreaterThan(0);
    RuleFor(a => a.SportId).GreaterThan(0);
    RuleFor(a => a)
      .MustAsync(HasNoDuplicateRegistration) 
      .WithMessage("The person is already registered for the selected sport at this event");
  }

  private async Task<bool> HasNoDuplicateRegistration(RegistrationDTO model, CancellationToken cancellationToken)
  {
    bool alreadyRegistered = await ctx.Registrations
                                          .AnyAsync(r =>
                                             r.EventId == model.EventId &&
                                             r.PersonId == model.PersonId &&
                                             r.SportId == model.SportId &&
                                             r.Id != model.Id,
                                             cancellationToken);
    return !alreadyRegistered;
  }
}
