using EF_EN;
using Microsoft.Extensions.DependencyInjection;

const int productCode = 12345678;
ServiceProvider serviceProvider;

using (serviceProvider = DISetup.BuildDI())
{
  Demo.PrintCities(serviceProvider);
  Demo.AddProduct(serviceProvider, productCode);
  Demo.RaiseProductPrice(serviceProvider, productCode, percentage: 0.1m);
  Demo.DeleteProduct(serviceProvider, productCode);
}

