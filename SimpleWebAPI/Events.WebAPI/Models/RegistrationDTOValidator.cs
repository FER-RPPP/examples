using Events.EF.Data;
using Events.WebAPI.Models.Configuration;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace Events.WebAPI.Models;

public class RegistrationDTOValidator : AbstractValidator<RegistrationDTO>
{
  private readonly EventsContext ctx;
  private readonly EventSettings eventSettings;
  
  public RegistrationDTOValidator(EventsContext ctx, IOptionsSnapshot<EventSettings> eventSettings)
  {
    this.ctx = ctx;
    this.eventSettings = eventSettings.Value;

    RuleFor(a => a.EventId).GreaterThan(0);
    RuleFor(a => a.PersonId).GreaterThan(0);
    RuleFor(a => a.SportId).GreaterThan(0);
    
    RuleFor(a => a)
      .Cascade(CascadeMode.Stop)
      .MustAsync(HasNoDuplicateRegistration)
      .WithMessage("The person is already registered for the selected sport at this event")
      .MustAsync(IsBelowMaxSports)
      .WithMessage($"A person can be registered for at most {this.eventSettings.MaxSports} sports at the same event")
      .OverridePropertyName(nameof(RegistrationDTO.SportId));
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

  private async Task<bool> IsBelowMaxSports(RegistrationDTO model, CancellationToken cancellationToken)
  {    
    int otherSports = await ctx.Registrations
                               .CountAsync(r =>
                                  r.EventId == model.EventId &&
                                  r.PersonId == model.PersonId &&
                                  r.Id != model.Id,
                                  cancellationToken);
    return otherSports < eventSettings.MaxSports;
  }
}
