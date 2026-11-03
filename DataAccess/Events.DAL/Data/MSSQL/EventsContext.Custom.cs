using Microsoft.EntityFrameworkCore;

namespace Events.DAL.Data.MSSQL;

public partial class EventsContext
{
  partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
  {
    modelBuilder
      .HasDbFunction(typeof(DbJson).GetMethod(nameof(DbJson.JsonValue))!)
      .HasName("JSON_VALUE")
      .IsBuiltIn();
  }
}
