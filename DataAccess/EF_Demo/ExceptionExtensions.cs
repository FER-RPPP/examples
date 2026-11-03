using System.Text;

namespace EF_Demo;

public static class ExceptionExtensions
{
  public static string CompleteExceptionMessage(this Exception exc)
  {
    StringBuilder sb = new StringBuilder(exc.Message);
    while (exc.InnerException != null)
    {
      exc = exc.InnerException;
      sb.AppendLine();
      sb.Append(exc.Message);
    }
    return sb.ToString();
  }
}
