using Microsoft.EntityFrameworkCore;

namespace Events.DAL.Data.Postgres;

public partial class EventsContext
{
  partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
  {
    modelBuilder
      .HasDbFunction(typeof(DbJson).GetMethod(nameof(DbJson.JsonValue))!)
      .HasName("jsonb_extract_path_text")
      .IsBuiltIn();
  }
}
