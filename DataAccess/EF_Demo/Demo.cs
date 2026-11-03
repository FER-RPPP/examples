#if POSTGRES
using Events.DAL.Data.Postgres;
#else
using Events.DAL.Data.MSSQL;
#endif

using Events.DAL.Models;
using Events.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EF_Demo;

internal class Demo(IDbContextFactory<EventsContext> contextFactory, ILogger<Demo> logger)
{
  internal int? AddEvent(string eventName, DateOnly eventDate)
  {
    try
    {
      using var ctx = contextFactory.CreateDbContext();
      Event @event = new()
      {
        Name = eventName,
        EventDate = eventDate
      };
      ctx.Add(@event);
      ctx.SaveChanges();
      logger.LogInformation($"Event {eventName} successfully added with id {@event.Id}");
      return @event.Id;
    }
    catch (Exception exc)
    {
      logger.LogError($"Error adding event #{eventName}: {exc.CompleteExceptionMessage()}");
      return null;
    }
  }

  internal void DeleteEvent(int eventId)
  {
    try
    {
      using var ctx = contextFactory.CreateDbContext();
      Event? @event = ctx.Events.Find(eventId);
      if (@event != null)
      {
        ctx.Remove(@event);
        ctx.SaveChanges();
        logger.LogInformation($"Event {@event.Name} deleted");
      }
      else
      {
        logger.LogWarning($"Event #{eventId} does not exists");
      }
    }
    catch (Exception exc)
    {
      logger.LogError($"Error deleting event #{eventId}: {exc.CompleteExceptionMessage()}");
    }
  }
  
  internal void PostponeEvent(int eventId, int days)
  {
    try
    {
      using var ctx = contextFactory.CreateDbContext();
      Event? @event = ctx.Events.Find(eventId);
      if (@event != null)
      {
        @event.EventDate = @event.EventDate.AddDays(days);
        ctx.SaveChanges();
        logger.LogInformation($"Event {@event.Name} postoponed to {@event.EventDate:dd.MM.yyyy.}");
      }
      else
      {
        logger.LogWarning($"Event #{eventId} does not exists");
      }
    }
    catch (Exception exc)
    {
      logger.LogError($"Error trying to change evenbt #{eventId}: {exc.CompleteExceptionMessage()}");
    }
  }

  /// <summary>
  /// Print all people born in the provided year, ordered by country, and then by transliterated last name, and first name
  /// query is projected to an anonymous class
  /// </summary>
  internal void PrintPeople(int year)
  {
#if POSTGRES
    const string hrPath = "hr";
#else
    const string hrPath = "$.hr";
#endif

    using var ctx = contextFactory.CreateDbContext();
    var query = ctx.People
                   .Where(p => p.BirthDate.Year == year)
                   .OrderBy(p => p.CountryCodeNavigation.Name)
                   .ThenBy(p => p.LastNameTranscription)
                   .ThenBy(p => p.FirstNameTranscription)
                   .Select(p => new
                   {
                     p.FirstName, p.LastName,
                     p.FirstNameTranscription, p.LastNameTranscription,
                     p.BirthDate,
                     Country = p.CountryCodeNavigation.Name,
                     CountryHr = DbJson.JsonValue(p.CountryCodeNavigation.Translations, hrPath)
                   });
    foreach (var p in query)
    {
      Console.WriteLine($"{p.FirstName} {p.LastName} ({p.FirstNameTranscription} {p.LastNameTranscription}), {p.Country} / {p.CountryHr} {p.BirthDate:dd.MM.yyyy.}");
    }
  }
}
