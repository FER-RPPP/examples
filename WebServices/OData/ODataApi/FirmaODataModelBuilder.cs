using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using DTOs = ODataApi.Contract;

namespace ODataApi
{
  public static class FirmaODataModelBuilder
  {
    public static IEdmModel GetEdmModel()
    {
      //definirati sve one na koje ide upit (bilo direktno ili kroz expand)
      //za one na koje se radi expand, samo definirati tip i klju

      var builder = new ODataConventionModelBuilder();

      builder.EntitySet<DTOs.Mjesto>("Mjesto")
                        .EntityType
                        .Filter() // Allow for the $filter Command
                        .Count() // Allow for the $count Command                     
                        .OrderBy() // Allow for the $orderby Command
                        .Page() // Allow for the $top and $skip Commands
                        .Select();// Allow for the $select Command;

      builder.EntitySet<EFModel.Dokument>("Dokument")                      
                      .EntityType.HasKey(d => d.IdDokumenta)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand() // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select() // Allow for the $select Command                                                                  
                      .HasMany(x => x.Stavka);

      builder.EntitySet<EFModel.Artikl>("Artikl")
                      .EntityType.HasKey(d => d.SifArtikla)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command
                      .Expand() // Allow for the $expand Command
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select() // Allow for the $select Command                                                                  
                      .HasMany(x => x.Stavka);

      builder.EntityType<EFModel.Stavka>()
             .HasKey(s => s.IdStavke);


      var partnerBuilder = builder.EntitySet<EFModel.Partner>("Partner")
                                  .EntityType.HasKey(d => d.IdPartnera)
                                  .Filter() // Allow for the $filter Command
                                  .Count() // Allow for the $count Command
                                  .Expand() // Allow for the $expand Command
                                  .OrderBy() // Allow for the $orderby Command
                                  .Page() // Allow for the $top and $skip Commands
                                  .Select(); // Allow for the $select Command


      partnerBuilder.HasOptional(p => p.Tvrtka);
      partnerBuilder.HasOptional(p => p.Osoba);


      builder.EntitySet<EFModel.Osoba>("Osoba")
                      .EntityType.HasKey(d => d.IdOsobe)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command                      
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select(); // Allow for the $select Command
                                 // 
      builder.EntitySet<EFModel.Tvrtka>("Tvrtka")
                      .EntityType.HasKey(d => d.IdTvrtke)
                      .Filter() // Allow for the $filter Command
                      .Count() // Allow for the $count Command                      
                      .OrderBy() // Allow for the $orderby Command
                      .Page() // Allow for the $top and $skip Commands
                      .Select(); // Allow for the $select Command 

      return builder.GetEdmModel();
    }
  }
}
