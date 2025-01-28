using System.Collections.Generic;

namespace WebApi.Models.JTable
{
  public class TableRecords<T> : JTableAjaxResult
  {
    public List<T> Records { get; set; }
    public int TotalRecordCount { get; set; }
    public TableRecords(int totalCount, List<T> records) : base()
    {
      TotalRecordCount = totalCount;
      Records = records;
    }    
  }
}
