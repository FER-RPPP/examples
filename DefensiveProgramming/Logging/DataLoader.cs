using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Logging;

public class DataLoader : IDataLoader
{
  private readonly ILogger<DataLoader> logger;

  public DataLoader(ILogger<DataLoader> logger)
  {
    this.logger = logger;
  }

  public List<(string, DateOnly)> LoadData(string filename)
  {
    List<(string, DateOnly)> list = new();
    if (!File.Exists(filename))
    {
      string message = $"File {filename} does not exist";
      logger.LogError(message);
      throw new FileNotFoundException(message);
    }
    else
    {
      //use File.ReadAllLines for small files
      using (var reader = new StreamReader(File.OpenRead(filename)))
      {
        string? line;
        while((line = reader.ReadLine()) != null) {
          string pattern = "[0-3]?[0-9].[0-3]?[0-9].[0-9]{4}.";
          var match = Regex.Match(line, pattern);
          if (match.Success)
          {
            string datetext = line.Substring(match.Index, match.Length);
            if (DateOnly.TryParseExact(datetext, "d.M.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
            {
              var item = (line.Remove(match.Index, match.Length).Trim(), date);
              logger.LogTrace(item.ToString());
              list.Add(item);
            }
            else
            {
              logger.LogWarning("Invalid date in line: " + line);
            }
          }
          else
          {
            logger.LogWarning("No date found in line: " + line);
          }
        }
      }
    }
    return list;
  }  
}
