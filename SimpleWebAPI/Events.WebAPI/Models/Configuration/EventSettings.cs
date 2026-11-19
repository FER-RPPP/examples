using System.ComponentModel.DataAnnotations;

namespace Events.WebAPI.Models.Configuration;

public class EventSettings
{  
  [Range(1, int.MaxValue, ErrorMessage = "EventSettings:MaxSports must be configured as a positive number.")]
  public int MaxSports { get; set; }
}
