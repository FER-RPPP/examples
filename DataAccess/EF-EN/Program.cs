using EF_EN;
using Microsoft.Extensions.DependencyInjection;

const int productCode = 12345678;

using ServiceProvider serviceProvider = DISetup.BuildDI();

Demo.PrintCities(serviceProvider);
Util.EnterToContinue();

Demo.AddProduct(serviceProvider, productCode);
Util.EnterToContinue();

Demo.RaiseProductPrice(serviceProvider, productCode, percentage: 0.1m);
Util.EnterToContinue();

Demo.DeleteProduct(serviceProvider, productCode);
Util.EnterToContinue(clearConsole: false);