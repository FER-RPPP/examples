using System.Text;

namespace EF_EN;

public static class ExceptionExtensions
{
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
