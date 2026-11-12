using Sieve.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Events.WebAPI.Models;

public class EventDTO
{
  [Sieve(CanSort = true)]
  public int Id { get; set; }

  [Sieve(CanFilter = true, CanSort = true)]
  [Required, MaxLength(150)]
  public string Name { get; set; } = string.Empty;

  [Sieve(CanFilter = true, CanSort = true)]
  [Required]
  public DateOnly EventDate { get; set; }

  [Sieve(CanSort = true)]
  public int RegistrationsCount { get; set; }
}
