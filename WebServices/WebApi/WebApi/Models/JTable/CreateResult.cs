namespace WebApi.Models.JTable;

public class CreateResult(object record) : JTableAjaxResult
{ 
  public object Record { get; set; } = record;
}
