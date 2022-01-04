using Microsoft.EntityFrameworkCore;

namespace EFModel
{
  public partial class FirmaContext
  {
    public virtual DbSet<ViewPartner> vw_Partner { get; set; }   
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ViewPartner>(entity => {
        entity.HasNoKey();    
      });      
    }
  }
}