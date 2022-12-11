using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace MVC.Models
{
  public partial class FirmaContext
  {
    public virtual DbSet<ViewPartner> vw_Partner { get; set; }
    public virtual DbSet<ViewDokumentInfo> vw_Dokumenti { get; set; }

    //The FromExpression call in the CLR function body allows for the function to be used instead of a regular DbSet.
    public IQueryable<StavkaDenorm> NajveceKupnje(int count) =>
      FromExpression(() => NajveceKupnje(count));

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<ViewPartner>(entity => {
        entity.HasNoKey();
        //entity.ToView("vw_Partner");
      });

      modelBuilder.Entity<ViewDokumentInfo>(entity => {
        entity.HasNoKey();
        //entity.ToView("vw_Dokumenti"); //u slučaju da se DbSet svojstvo zove drugačije
      });

      modelBuilder.Entity<StavkaDenorm>(entity => {
        entity.HasNoKey();       
      });

      modelBuilder.HasDbFunction(typeof(FirmaContext).GetMethod(nameof(NajveceKupnje), new[] { typeof(int) }))
                  .HasName("fn_NajveceKupnje");
                  
    }
  }
}