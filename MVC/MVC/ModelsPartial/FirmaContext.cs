using Microsoft.EntityFrameworkCore;
using MVC.ModelsPartial;

namespace MVC.Models;

public partial class FirmaContext
{
  public virtual DbSet<ViewPartner> vw_Partner { get; set; }
  public virtual DbSet<ViewDokumentInfo> vw_Dokumenti { get; set; }

  //The FromExpression call in the CLR function body allows for the function to be used instead of a regular DbSet.
  [DbFunction("fn_NajboljiPartneri", "dbo")]
  public IQueryable<NajboljiPartner> NajboljiPartneri(int godina, int broj) =>
    FromExpression(() => NajboljiPartneri(godina, broj));

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<ViewPartner>(entity => {
      entity.HasKey(p => p.IdPartnera);  //potreban zbog joina kod reporta
      //entity.HasNoKey();
      //entity.ToView("vw_Partner");
    });

    modelBuilder.Entity<ViewDokumentInfo>(entity => {
      entity.HasNoKey();
      //entity.ToView("vw_Dokumenti"); //u slučaju da se DbSet svojstvo zove drugačije
    });

    modelBuilder.Entity<NajboljiPartner>(entity => {
      entity.HasNoKey();
    });
  }
}