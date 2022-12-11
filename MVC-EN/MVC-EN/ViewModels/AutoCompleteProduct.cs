using System.Text.Json.Serialization;

namespace MVC_EN.ViewModels;

public class AutoCompleteProduct
{
  [JsonPropertyName("label")]
  public string Label { get; set; }
  [JsonPropertyName("id")]
  public int Id { get; set; }
  [JsonPropertyName("price")]
  public decimal Price { get; set; }
  public AutoCompleteProduct() { }
  public AutoCompleteProduct(int id, string label, decimal price)
  {
    Id = id;
    Label = label;
    Price = price;
  }
}
