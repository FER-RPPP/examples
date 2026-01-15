using System.Text;

namespace WebApi.Util.Extensions
{
  /// <summary>
  /// Class with useful extensions for exceptions handling
  /// </summary>
  public static class ExceptionExtensions
  {
    /// <summary>
    /// return complete hierarchy of an exception. It checks whether the exception has inner exception,
    /// and if it has, then it appends inner exception message.
    /// Then it looks for inner exception of the inner exceptions, and so on.
    /// </summary>
    /// <param name="exc">Exception which message hiearchy should be obtained</param>
    /// <returns>String containing all exception hierarchy messages</returns>
    public static string CompleteExceptionMessage(this Exception? exc)
    {
      StringBuilder sb = new();
      while (exc != null)
      {
        sb.AppendLine(exc.Message);
        exc = exc.InnerException;
      }
      return sb.ToString();
    }
  }
}
