﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace MVC.Models
{
  public partial class FirmaContext
  {
    public virtual DbSet<ViewPartner> vw_Partner { get; set; }
    public virtual DbSet<ViewDokumentInfo> vw_Dokumenti { get; set; }

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
    }
  }
}