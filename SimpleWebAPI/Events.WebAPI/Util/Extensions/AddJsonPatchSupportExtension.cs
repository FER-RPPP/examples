using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace Events.WebAPI.Util.Extensions;

public static class AddJsonPatchSupportExtension
{
  public static void AddJsonPatchSupport(this MvcOptions options)
  {
    options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
  }

  // Add Newtonsoft only to the JSON Patch formatter so the rest of the API keeps System.Text.Json.
  private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
  {
    var builder = new ServiceCollection()
      .AddLogging()
      .AddControllers()
      .AddNewtonsoftJson()
      .Services
      .BuildServiceProvider();

    return builder
      .GetRequiredService<IOptions<MvcOptions>>()
      .Value
      .InputFormatters
      .OfType<NewtonsoftJsonPatchInputFormatter>()
      .First();
  }
}
