using EFModel; //referenciran jednostavni radi - načelno treba generirati klase temeljem scheme
using DTOs = ODataApi.Contract; //dijeljeni projekt, samo čisti objekti
using Simple.OData.Client;
using System;
using System.Threading.Tasks;

namespace ConsoleODataClient
{
  class Program
  {
    const string url = "https://localhost:44343/odata";
    static async Task Main(string[] args)
    {
      ODataClientSettings settings = new ODataClientSettings(new Uri(url));
      settings.IgnoreResourceNotFoundException = true;
      settings.OnTrace = (x, y) => Console.WriteLine(string.Format(x, y));

      var client = new ODataClient(settings);

      await DemoTraziMjesta(client);
      await DemoMjesto(client);
      await DemoDokumenti(client);
    }

    private static async Task DemoTraziMjesta(ODataClient client)
    {
      Console.WriteLine("Broj svih mjesta: " + await client.For<DTOs.Mjesto>().Count().FindScalarAsync<int>());

      Console.WriteLine("Mjesto s IdMjesta = 35413");
      await IspisiMjesto(client, 35413);

      Console.WriteLine("sva mjesta u Hrvatskoj s poštanskim brojem između 10000 i 10500 koja ne sadrže riječ Zagreb:");
      //sva mjesta u Hrvatskoj s poštanskim brojem između 10000 i 10500 koja ne sadrže riječ Zagreb
      var mjesta = await client.For<DTOs.Mjesto>()
                               .Filter(m => m.OznDrzave == "HR")
                               .Filter(m => m.PostBrojMjesta >= 10000 && m.PostBrojMjesta < 10100)
                               .Filter(m => !m.NazivMjesta.Contains("Zagreb"))
                               .Select(m => new { m.PostBrojMjesta, m.NazivMjesta })
                               .OrderBy(m => m.PostBrojMjesta)
                               .FindEntriesAsync();

      foreach (var mjesto in mjesta)
      {
        Console.WriteLine($"{mjesto.PostBrojMjesta} {mjesto.NazivMjesta}");
      }
    }

    private static async Task DemoMjesto(ODataClient client)
    {
      var novoMjesto = new DTOs.Mjesto
      {
        PostBrojMjesta = 59999,
        NazivMjesta = "Test mjesto",
        OznDrzave = "HR",
        PostNazivMjesta = "Test mjesto"
      };

      try
      {
        var mjesto = await client.For<DTOs.Mjesto>()
                                  .Set(novoMjesto)
                                  .InsertEntryAsync();
        Console.WriteLine("Mjesto dodano pod ključem: " + mjesto.IdMjesta);
        await IspisiMjesto(client, mjesto.IdMjesta);

        mjesto.NazivMjesta += "_";
        mjesto = await client.For<DTOs.Mjesto>()
                             .Key(mjesto.IdMjesta)
                             .Set(new { NazivMjesta = "_NoviTestNaziv_" })
                             .UpdateEntryAsync();
        Console.WriteLine("Mjesto ažurirano");
        await IspisiMjesto(client, mjesto.IdMjesta);



        await client.For<DTOs.Mjesto>()
                    .Key(mjesto.IdMjesta)
                    .DeleteEntryAsync();

        Console.WriteLine("Mjesto obrisano");
      }
      catch (WebRequestException exc)
      {
        Console.WriteLine(exc.Message);
        Console.WriteLine(exc.Response);
      }
    }

    private static async Task DemoDokumenti(ODataClient client)
    {
      Console.WriteLine("Broj svih dokumenata: " + await client.For<Dokument>().Count().FindScalarAsync<int>());

      //prva 3 dokumenata kojima prezime osobe počinje sa Stipan i nastali su prije 2016. godine
      //i imaju iznos veći od 10000
      var dokumenti = await client.For<Dokument>()
                                  .Filter(d => d.IdPartneraNavigation.Osoba.PrezimeOsobe.StartsWith("Stipan"))
                                  .Filter(d => d.DatDokumenta < new DateTime(2016, 1, 1))
                                  .Filter(d => d.IznosDokumenta > 10000)
                                  .OrderByDescending(d => d.IznosDokumenta)
                                  .Expand(d => d.IdPartneraNavigation.Osoba)                                  
                                  .Select(d => new {d.IdDokumenta, d.DatDokumenta, d.IznosDokumenta,
                                                    d.IdPartneraNavigation.Osoba.ImeOsobe,
                                                    d.IdPartneraNavigation.Osoba.PrezimeOsobe,
                                                    })                                  
                                  .Top(3)                                  
                                  .FindEntriesAsync();
      foreach (var d in dokumenti)
      {        
        Console.WriteLine($"{d.IdDokumenta}/{d.DatDokumenta:d.M.yyyy} {d.IdPartneraNavigation.Osoba.ImeOsobe} {d.IdPartneraNavigation.Osoba.PrezimeOsobe}");        
      }

      dokumenti = await client.For<Dokument>()
                              .Filter(d => d.IdPartneraNavigation.Osoba.PrezimeOsobe.StartsWith("Stipan"))
                              .Filter(d => d.DatDokumenta < new DateTime(2016, 1, 1))
                              .Filter(d => d.IznosDokumenta > 10000)
                              .Expand(d => d.Stavka)
                              .OrderByDescending(d => d.IznosDokumenta)
                              .Select(d => new {
                                d.IdDokumenta,
                                d.DatDokumenta,  
                                d.Stavka
                              })
                              .Top(3)
                              .FindEntriesAsync();
      foreach (var d in dokumenti)
      {
        Console.WriteLine($"{d.IdDokumenta}/{d.DatDokumenta:d.M.yyyy}");
        foreach (var s in d.Stavka)
        {
          Console.WriteLine($"\t {s.KolArtikla:C2} x {s.SifArtikla} {s.JedCijArtikla:N2}");
        }
      }
    }

    private static async Task IspisiMjesto(ODataClient client, int key)
    {
      var mjesto = await client.For<DTOs.Mjesto>()
                               .Key(key)
                               .FindEntryAsync();
      if (mjesto != null)
      {
        Console.WriteLine($"IdMjesta: {mjesto.IdMjesta} {mjesto.OznDrzave}-{mjesto.PostBrojMjesta} {mjesto.NazivMjesta}");
      }
      else
      {
        Console.WriteLine($"Mjesto s id-om {key} ne postoji");
      }
    }
      
  }
}
