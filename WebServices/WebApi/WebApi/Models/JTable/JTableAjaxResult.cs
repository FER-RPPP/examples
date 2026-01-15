namespace WebApi.Models.JTable;

public class JTableAjaxResult
{
  public string Result { get; private set; }
  public string? Message { get; private set; }

  protected JTableAjaxResult()
  {
    Result = "OK";      
  }

  private JTableAjaxResult(string errorMessage)
  {
    Result = "ERROR";
    Message = errorMessage;
  }

  public static JTableAjaxResult OK => new JTableAjaxResult();
  public static JTableAjaxResult Error(string errorMessage) => new JTableAjaxResult(errorMessage);
    
}
