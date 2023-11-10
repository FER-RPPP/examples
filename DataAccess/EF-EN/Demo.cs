using EF_EN.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EF_EN;

internal class Demo
{
  internal static void AddProduct(IServiceProvider serviceProvider, int productCode)
  {
    ILogger? logger = serviceProvider.GetService<ILogger<Demo>>();
    try
    {
      using var ctx = serviceProvider.GetRequiredService<FirmContext>();
      var product = new Product()
      {
        ProductNumber = productCode,
        Price = 100m,
        UnitName = "piece",
        ProductName = "Cheap Phone v1"
      };
      ctx.Products.Add(product);  //context.Set<Product>().Add(product);
      ctx.SaveChanges();
      logger?.LogInformation($"Product #{product.ProductNumber} successfully added");
    }
    catch (Exception exc)
    {
      logger?.LogError($"Error adding product #{productCode}: {exc.CompleteExceptionMessage()}");
    }
  }

  internal static void DeleteProduct(ServiceProvider serviceProvider, int productCode)
  {
    ILogger? logger = serviceProvider.GetService<ILogger<Demo>>();
    try
    {
      using var ctx = serviceProvider.GetRequiredService<FirmContext>();
      //Product product = ctx.Find<Product>(productCode);
      Product? product = ctx.Products.Find(productCode);
      if (product != null)
      {
        ctx.Products.Remove(product);
        //context.Entry(product).State = EntityState.Deleted;
        ctx.SaveChanges();
        logger?.LogInformation($"Product #{product.ProductNumber} deleted");
      }
      else
      {
        logger?.LogWarning($"Product #{productCode} does not exists");
      }
    }
    catch (Exception exc)
    {
      logger?.LogError($"Error deleting product #{productCode}: {exc.CompleteExceptionMessage()}");
    }
  }
  
  internal static void RaiseProductPrice(ServiceProvider serviceProvider, int productCode, decimal percentage)
  {
    ILogger? logger = serviceProvider.GetService<ILogger<Demo>>();
    try
    {
      using var ctx = serviceProvider.GetRequiredService<FirmContext>();
      Product? product = ctx.Products.Find(productCode);
      if (product != null)
      {
        product.Price *= 1 + percentage;
        ctx.SaveChanges();
        logger?.LogInformation("Price changed");
      }
      else
      {
        logger?.LogWarning($"Product #{productCode} does not exists");
      }
    }
    catch (Exception exc)
    {
      logger?.LogError($"Error trying to change product #{productCode} price: {exc.CompleteExceptionMessage()}");
    }
  }

  /// <summary>
  /// Print all cities containing word Velika, 
  /// query is projected to an anonymous class
  /// with city id, postal code, and country name
  /// </summary>
  /// <param name="ctx"></param>
  internal static void PrintCities(IServiceProvider serviceProvider)
  {
    using var ctx = serviceProvider.GetRequiredService<FirmContext>();
    var query = ctx.Cities
                   .Where(c => c.CityName.Contains("Velika"))
                   .OrderBy(c => c.CountryCode)
                   .ThenBy(c => c.PostalCode)
                   .Select(c => new
                   {
                     c.CityName,
                     c.PostalCode,
                     Country = c.CountryCodeNavigation.CountryName
                   });
    foreach (var c in query)
    {
      Console.WriteLine($"{c.PostalCode} {c.CityName} ({c.Country})");
    }
  }
}
