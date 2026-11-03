using EF_Demo;
using Events.DAL;
using Microsoft.Extensions.DependencyInjection;

Console.OutputEncoding = System.Text.Encoding.UTF8;

using var host = DISetup.BuildHost(args);
Demo demo = host.Services.GetRequiredService<Demo>();

demo.PrintPeople(1976);
Util.EnterToContinue();

int? eventId = demo.AddEvent("Spring festival", new DateOnly(2026, 4, 30));
Util.EnterToContinue();

if (eventId.HasValue)
{
  demo.PostponeEvent(eventId.Value, days: 5);
  Util.EnterToContinue();

  demo.DeleteEvent(eventId.Value);
  Util.EnterToContinue(clearConsole: false);
}
