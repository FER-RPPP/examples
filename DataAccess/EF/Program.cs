using EF;
using Microsoft.Extensions.DependencyInjection;

const int productCode = 12345678;
using ServiceProvider serviceProvider = DISetup.BuildDI();

Demo.AddProduct(serviceProvider, productCode);
Demo.ChangeProductPrice(serviceProvider, productCode);
Demo.DeleteProduct(serviceProvider, productCode);
Demo.PrintMostExpensives(serviceProvider);
Demo.PrintMostExpensivesAnonymous(serviceProvider);