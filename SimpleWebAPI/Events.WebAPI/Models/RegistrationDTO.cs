using Sieve.Attributes;

namespace Events.WebAPI.Models;

public class RegistrationDTO
{
  [Sieve(CanSort = true)]
  public int Id { get; set; }

  [Sieve(CanFilter = true, CanSort = true)]
  public int EventId { get; set; }

  [Sieve(CanFilter = true, CanSort = true)]
  public int PersonId { get; set; }

  [Sieve(CanFilter = true, CanSort = true)]
  public int SportId { get; set; }

  [Sieve(CanSort = true)]
  public DateTime RegisteredAt { get; set; }

  [Sieve(CanFilter = true, CanSort = true)]
  public string PersonName { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string PersonFirstNameTranscription { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string PersonLastNameTranscription { get; set; } = string.Empty;

  [Sieve(CanFilter = true)]
  public string CountryCode { get; set; } = string.Empty;

  [Sieve(CanSort = true)]
  public string CountryName { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string SportName { get; set; } = string.Empty;
  [Sieve(CanFilter = true, CanSort = true)]
  public string EventName { get; set; } = string.Empty;
}
