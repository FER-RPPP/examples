﻿using EF.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EF;

internal class Demo
{
  internal static void AddProduct(IServiceProvider serviceProvider, int productCode)
  {
    try
    {
      using (var context = serviceProvider.GetRequiredService<FirmaContext>())
      {
        Artikl artikl = new Artikl()
        {
          SifArtikla = productCode,
          CijArtikla = 500m,
          JedMjere = "kom",
          NazArtikla = "Cheap Phone v1"
        };
        context.Artikl.Add(artikl);  //context.Set<Artikl>().Add(artikl);
        context.SaveChanges();
        Console.WriteLine($"Product #{artikl.SifArtikla} successfully added");
      }
    }
    catch (Exception exc)
    {
      Console.WriteLine($"Error adding product #{productCode}: {exc.CompleteExceptionMessage()}");
    }
  }

  internal static void DeleteProduct(IServiceProvider serviceProvider, int productCode)
  {
    try
    {
      using (var context = serviceProvider.GetRequiredService<FirmaContext>())
      {
        //Artikl? artikl = context.Find<Artikl>(sifraArtikla);
        Artikl? artikl = context.Artikl.Find(productCode);
        if (artikl != null) 
        {
          context.Artikl.Remove(artikl);
          //context.Entry(artikl).State = EntityState.Deleted;
          context.SaveChanges();
          Console.WriteLine($"Product #{artikl.SifArtikla} deleted");
        }
        else
        {
          Console.WriteLine($"Product #{productCode} does not exists");
        }
      }
    }
    catch (Exception exc)
    {
      Console.WriteLine($"Error deleting product #{productCode}: {exc.CompleteExceptionMessage()}");
    }
  }

  internal static void ChangeProductPrice(IServiceProvider serviceProvider, int productCode)
  {
    try
    {
      using (var context = serviceProvider.GetRequiredService<FirmaContext>())
      {
        Artikl? artikl = context.Artikl.Find(productCode);
        if (artikl != null)
        {
          artikl.CijArtikla = 600m;
          context.SaveChanges();
          Console.WriteLine("Price changed");
        }
        else
        {
          Console.WriteLine($"Product #{productCode} does not exists");
        }
      }
    }
    catch (Exception exc)
    {
      Console.WriteLine($"Error trying to change product #{productCode} price: {exc.CompleteExceptionMessage()}");
    }
  }


  internal static void PrintMostExpensives(IServiceProvider serviceProvider)
  {
    Console.WriteLine("How many most expensive products to print?");
    int n = int.Parse(Console.ReadLine());
    using (var context = serviceProvider.GetRequiredService<FirmaContext>())
    {
      var query = context.Artikl
                         .Include(a => a.Stavka)
                         .AsNoTracking()
                         .OrderByDescending(a => a.CijArtikla)
                         .Take(n);
      int cnt = 0;
      foreach (var product in query)
      {
        Console.WriteLine("{0}. {1} - {2:C2} ({3})", ++cnt, product.NazArtikla, product.CijArtikla, product.Stavka.Count);
      }
    }
  }

  internal static void PrintMostExpensivesAnonymous(IServiceProvider serviceProvider)
  {
    Console.WriteLine("How many most expensive products to print?");
    int n = int.Parse(Console.ReadLine());
    using (var context = serviceProvider.GetRequiredService<FirmaContext>())
    {
      var query = context.Artikl
                         .OrderByDescending(a => a.CijArtikla)
                         .Select(a => new { a.NazArtikla, a.CijArtikla, Sales = a.Stavka.Count })
                         .Take(n);
      int cnt = 0;
      foreach (var product in query)
      {
        Console.WriteLine("{0}. {1} - {2:C2} ({3})", ++cnt, product.NazArtikla, product.CijArtikla, product.Sales);
      }
    }
  }
}
