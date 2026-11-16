using Sieve.Attributes;

namespace Events.WebAPI.Models;

public class PersonDTO
{
  [Sieve(CanFilter = true, CanSort = true)]
  public int Id { get; set; }

  [Sieve(CanFilter = true, CanSort = true)]
  public string? FirstName { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string? LastName { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string FirstNameTranscription { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string LastNameTranscription { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string? AddressLine { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string? PostalCode { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string? City { get; set; } = string.Empty;
  
  [Sieve(CanFilter = true, CanSort = true)]
  public string? AddressCountry { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string? Email { get; set; } = string.Empty;
  
  [Sieve(CanFilter = true, CanSort = true)] 
  public string? ContactPhone { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public DateOnly BirthDate { get; set; }

  [Sieve(CanFilter = true, CanSort = true)]
  public string DocumentNumber { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string CountryCode { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string? CountryName { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public string? FullNameTranscription { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  public int RegistrationsCount { get; set; }
}
