namespace WebApi.Models.JTable;

public class OptionsResult(List<TextValue> options) : JTableAjaxResult
{  
  public List<TextValue> Options { get; set; } = options;
}
