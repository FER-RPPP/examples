using Microsoft.EntityFrameworkCore;

namespace MVC_EN.Models;

public partial class FirmContext
{
  public virtual DbSet<ViewPartner> vw_Partners { get; set; }
  public virtual DbSet<ViewDocument> vw_Documents { get; set; }
  
  //The FromExpression call in the CLR function body allows for the function to be used instead of a regular DbSet.
  public IQueryable<ItemDenorm> BiggestPurchases(int count) => 
    FromExpression(() => BiggestPurchases(count));

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<ViewPartner>(entity => {
      entity.HasNoKey();
      //entity.ToView("vw_Partners");
    });

    modelBuilder.Entity<ViewDocument>(entity => {
      entity.HasNoKey();
      //entity.ToView("vw_Documents");
    });


    modelBuilder.Entity<ItemDenorm>(entity => {
      entity.HasNoKey();      
    });

    modelBuilder.HasDbFunction(typeof(FirmContext).GetMethod(nameof(BiggestPurchases), new[] { typeof(int) }))
                .HasName("fn_BiggestPurchases");
  }
}