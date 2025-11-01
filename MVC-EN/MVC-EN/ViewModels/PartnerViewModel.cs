using System.ComponentModel.DataAnnotations;

namespace MVC_EN.ViewModels;

public class PartnerViewModel : IValidatableObject
{
  public int PartnerId { get; set; }
  [Required]
  [RegularExpression("[PC]")]
  public string? PartnerType { get; set; }
  [Display(Name = "Last name")]
  public string? PersonLastName { get; set; }
  [Display(Name = "First name")]
  public string? PersonFirstName { get; set; }
  [Display(Name = "Registration Number")]
  public string? RegistrationNumber { get; set; }
  [Display(Name = "Company Name")]
  public string? CompanyName { get; set; }
  [Required]
  [RegularExpression("[0-9]{11}")]
  [Display(Name = "VAT Number")] public string? VATNumber { get; set; }
  [Display(Name = "Residence Address")] public string? ResidenceAddress { get; set; }
  [Display(Name = "Residence City")] public int? ResidenceCityId { get; set; }
  public string? ResidenceCityName { get; set; }
  [Display(Name = "Shipment Address")] public string? ShipmentAddress { get; set; }
  [Display(Name = "Shipment City")] public int? ShipmentCityId { get; set; }
  public string? ShipmentCityName { get; set; }

  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    if (PartnerType == "P")
    {
      if (string.IsNullOrWhiteSpace(PersonFirstName)) 
      { 
        yield return new ValidationResult("Enter person's first name", new[] { nameof(PersonFirstName) }); 
      }
      if (string.IsNullOrWhiteSpace(PersonLastName))
      {
        yield return new ValidationResult("Enter person's last name", new[] { nameof(PersonLastName) });
      }
    }
    else if (PartnerType == "C")
    {
      if (string.IsNullOrWhiteSpace(CompanyName))
      {
        yield return new ValidationResult("Enter company name", new[] { nameof(CompanyName) });
      }
      if (string.IsNullOrWhiteSpace(RegistrationNumber))
      {
        yield return new ValidationResult("Enter company registration number", new[] { nameof(RegistrationNumber) });
      }
    }

  }
}
