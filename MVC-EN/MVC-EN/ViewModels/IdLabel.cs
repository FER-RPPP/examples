using System.Text.Json.Serialization;

namespace MVC_EN.ViewModels;

public class IdLabel
{
  [JsonPropertyName("label")]
  public string Label { get; set; } = string.Empty;

  [JsonPropertyName("id")]
  public int Id { get; set; } 
}
