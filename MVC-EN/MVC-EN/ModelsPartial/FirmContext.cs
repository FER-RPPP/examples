using Microsoft.EntityFrameworkCore;

namespace MVC_EN.Models;

public partial class FirmContext
{
  public virtual DbSet<ViewPartner> vw_Partners { get; set; }
  public virtual DbSet<ViewDocument> vw_Documents { get; set; }

  //The FromExpression call in the CLR function body allows for the function to be used instead of a regular DbSet.
  [DbFunction("fn_BestPartners", "dbo")]
  public IQueryable<BestPartner> BestPartners(int year, int count) =>
    FromExpression(() => BestPartners(year, count));

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<ViewPartner>(entity => {      
      entity.HasKey(p => p.PartnerId);      
    });

    modelBuilder.Entity<ViewDocument>(entity => {
      entity.HasNoKey();
      //entity.ToView("vw_Documents");
    });


    modelBuilder.Entity<BestPartner>(entity => {
      entity.HasNoKey();      
    });  
  }
}