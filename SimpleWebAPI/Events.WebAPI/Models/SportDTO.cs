using System.ComponentModel.DataAnnotations;

namespace Events.WebAPI.Models;

public class SportDTO
{
  public int Id { get; set; }

  [Required, MaxLength(100)]
  public string Name { get; set; } = string.Empty;
}
