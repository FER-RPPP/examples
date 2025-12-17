using System.Text.Json.Serialization;

namespace MVC.ViewModels;

public class IdLabel
{
  [JsonPropertyName("label")]
  public string Label { get; set; } = string.Empty;

  [JsonPropertyName("id")]
  public int Id { get; set; } 
}
