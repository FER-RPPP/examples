namespace Contract.DTOs;

public class TextValue<V>
{
  public required V Value { get; set; }
  public required string Text { get; set; }    
}
