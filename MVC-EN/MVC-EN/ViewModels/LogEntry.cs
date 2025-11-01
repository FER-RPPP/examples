namespace MVC_EN.ViewModels;

public class LogEntry
{
  public DateTime Time { get; set; }
  public int Id { get; set; }
  public required string Controller { get; set; }
  public required string Level { get; set; }
  public required string Message { get; set; }
  public required string Url { get; set; }
  public required string Action { get; set; }

  internal static LogEntry FromString(string text)
  {
    string[] arr = text.Split('|');
    LogEntry entry = new LogEntry
    {
      Time = DateTime.ParseExact(arr[0], "yyyy-MM-dd HH:mm:ss.ffff", System.Globalization.CultureInfo.InvariantCulture),
      Id = string.IsNullOrWhiteSpace(arr[1]) ? 0 : int.Parse(arr[1]),
      Level = arr[2],
      Controller = arr[3],
      Message = arr[4],
      Url = arr[5].Substring(5), //url: 
      Action = arr[6].Substring(8) //action: 
    };

    return entry;
  }
}
