using System.Text;

namespace MVC_EN.Extensions;

/// <summary>
/// Class with extension methods for exceptions
/// </summary>
public static class ExceptionExtensions
{
  /// <summary>
  /// Unwraps exceptions and inner exceptions hierarchy, and returns in a single string
  /// Each inner exception is in a separate line.
  /// </summary>
  /// <param name="exc">exception which hierarhcy of inner exceptions must be returned</param>
  /// <returns>String in which each exception message are separated by new line</returns>
  public static string CompleteExceptionMessage(this Exception? exc)
  {
    StringBuilder sb = new StringBuilder();
    while (exc != null)
    {
      sb.AppendLine(exc.Message);
      exc = exc.InnerException;
    }
    return sb.ToString();
  }
}
