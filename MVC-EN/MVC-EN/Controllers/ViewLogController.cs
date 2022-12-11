using Microsoft.AspNetCore.Mvc;
using MVC_EN.ViewModels;

namespace MVC_EN.Controllers;

public class ViewLogController : Controller
{        
  public IActionResult Index()
  {      
    return View();
  }

  public async Task<IActionResult> Show(DateTime day)
  {
    List<LogEntry> list = new List<LogEntry>();
    ViewBag.Day = day;      
    string format = day.ToString("yyyy-MM-dd");
    string filename = Path.Combine(AppContext.BaseDirectory, $"logs/nlog-own-{format}.log");
    if (System.IO.File.Exists(filename))
    {
      string previousEntry = string.Empty;
      using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {          
        using (StreamReader reader = new StreamReader(fileStream))
        {
          string line;
          while ((line = await reader.ReadLineAsync()) != null)
          {
            if (line.StartsWith(format))
            {
              //new entry begins, add previous in the list
              if (previousEntry != string.Empty)
              {
                LogEntry logEntry = LogEntry.FromString(previousEntry);
                list.Add(logEntry);
              }
              previousEntry = line;
            }
            else
            {
              previousEntry += line;
            }
          }
        }
      }
      //add last one

      if (previousEntry != string.Empty)
      {
        LogEntry logEntry = LogEntry.FromString(previousEntry);
        list.Add(logEntry);
      }
    }
    list.Sort((a, b) => -a.Time.CompareTo(b.Time));
    return View(list);
  }
}
