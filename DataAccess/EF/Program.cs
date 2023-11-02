using EF;
using Microsoft.Extensions.DependencyInjection;

const int productCode = 12345678;
using ServiceProvider serviceProvider = DISetup.BuildDI();

Demo.AddProduct(serviceProvider, productCode);
Util.EnterToContinue();

Demo.ChangeProductPrice(serviceProvider, productCode);
Util.EnterToContinue();

Demo.DeleteProduct(serviceProvider, productCode);
Util.EnterToContinue();

Demo.PrintMostExpensives(serviceProvider);
Demo.PrintMostExpensivesAnonymous(serviceProvider);
Util.EnterToContinue(clearConsole: false);