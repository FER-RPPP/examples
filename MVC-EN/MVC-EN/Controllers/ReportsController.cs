using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using MVC_EN.Extensions;
using MVC_EN.Models;
using MVC_EN.ViewModels.PDFs;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace MVC_EN.Controllers;

public class ReportsController : Controller
{
  private readonly FirmContext ctx;  
  private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

  public ReportsController(FirmContext ctx)
  {
    this.ctx = ctx;    
  }

  public IActionResult Index()
  {
    return View();
  }

  #region Export in Excel
  public async Task<IActionResult> CountriesExcel()
  {
    var countries = await ctx.Countries
                             .AsNoTracking()
                             .OrderBy(d => d.CountryName)
                             .ToListAsync();
    byte[] content;
    using (ExcelPackage excel = new ExcelPackage())
    {
      excel.Workbook.Properties.Title = "Countries list";
      excel.Workbook.Properties.Author = "FER-ZPR";
      var worksheet = excel.Workbook.Worksheets.Add("Countries");

      //First add the headers
      worksheet.Cells[1, 1].Value = "Country Code";
      worksheet.Cells[1, 2].Value = "Country Name";
      worksheet.Cells[1, 3].Value = "ISO3";

      for (int i = 0; i < countries.Count; i++)
      {
        worksheet.Cells[i + 2, 1].Value = countries[i].CountryCode;
        worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        worksheet.Cells[i + 2, 2].Value = countries[i].CountryName;
        worksheet.Cells[i + 2, 3].Value = countries[i].CountryIso3;
      }

      worksheet.Cells[1, 1, countries.Count + 1, 4].AutoFitColumns();

      content = excel.GetAsByteArray();
    }
    return File(content, ExcelContentType, "countries.xlsx");
  }

  public async Task<IActionResult> DocumentsExcel()
  {
    var documents = await ctx.vw_Documents
                             .OrderByDescending(d => d.Amount)
                             .Take(10)
                             .ToListAsync();
    byte[] content;
    using (ExcelPackage excel = documents.CreateExcel("Documents"))
    {
      content = excel.GetAsByteArray();
    }
    return File(content, ExcelContentType, "documents.xlsx");
  }

  #endregion

  public async Task<IActionResult> Countries()
  {    
    var countries = await ctx.Countries
                             .AsNoTracking()
                             .OrderBy(d => d.CountryName)
                             .ToListAsync();
    var report = countries.CreateTableReport("Countries", AddColumnsForCountries, AddCountry);

    byte[] pdf = report.GeneratePdf();

    if (pdf != null)
    {
      Response.Headers.Append("content-disposition", "inline; filename=countries.pdf");
      return File(pdf, "application/pdf");
      //return File(pdf, "application/pdf", "countries.pdf"); //Opens save as dialog
    }
    else
    {
      return NotFound();
    }
  }

  public async Task<IActionResult> Products()
  {
    string title = "10 most expensive products that have a photo";
    var products = await ctx.Products                            
                            .Where(a => a.Photo != null)
                            .OrderByDescending(a => a.Price)
                            .Select(a => new
                            {
                              a.ProductNumber,
                              a.ProductName,
                              a.Price,
                              a.Photo
                            })
                            .Take(10)
                            .ToListAsync();

    var report = products.CreateTableReport(title,
      AddColumnsForProducts,
      (table, i, product, cellStyle) =>
      {
        table.Cell().Element(cellStyle).AlignRight().Text($"{i}.");
        table.Cell().Element(cellStyle).AlignCenter().MaxHeight(0.9f, Unit.Inch).MaxWidth(1, Unit.Inch).Image(product.Photo);
        table.Cell().Element(cellStyle).AlignLeft().PaddingLeft(5).Text(product.ProductNumber.ToString());
        table.Cell().Element(cellStyle).AlignCenter().Text(product.ProductName);
        table.Cell().Element(cellStyle).AlignRight().PaddingRight(5).Text(product.Price.ToString("N2"));
      },
      table =>
      {
        table.Cell().ColumnSpan(4).AlignRight().Text("TOTAL: ").Bold();
        table.Cell().PaddingRight(5).Text(products.Sum(a => a.Price).ToString("N2")).AlignRight().Bold();
      }
    );

    byte[] pdf = report.GeneratePdf();   
    if (pdf != null)
    {
      Response.Headers.Append("content-disposition", "inline; filename=products.pdf");
      return File(pdf, "application/pdf");
    }
    else
    {
      return NotFound();
    }
  }


  public async Task<IActionResult> Documents()
  {
    int n = 10;
    string title = $"Top {n} biggets purchases";
    var partners = ctx.vw_Partners;
    var documents = await ctx.Documents
                             .Join(partners, d => d.PartnerId, p => p.PartnerId, (d, p) => new Order
                             {
                               DocumentDate = d.DocumentDate,
                               DocumentId = d.DocumentId,
                               Amount = d.Amount,
                               PartnerName = p.PartnerName,
                               VatNumber = p.VatNumber,
                               Items = d.Items.Select(item => new OrderItem
                               {
                                 ItemId = item.ItemId,
                                 UnitPrice = item.UnitPrice,
                                 Quantity = item.Quantity,
                                 ProductName = item.ProductNumberNavigation.ProductName,
                                 Discount = item.Discount,
                                 ProductNumber = item.ProductNumber
                               })
                             })
                             .OrderByDescending(d => d.Amount)
                             .Take(n)
                             .ToListAsync();
   
    IDocument pdfDocument = new DocumentsPdf(documents,title, Url);
    byte[] pdf = pdfDocument.GeneratePdf();

    if (pdf != null)
    {
      Response.Headers.Append("content-disposition", "inline; filename=documents.pdf");
      return File(pdf, "application/pdf");
    }
    else
      return NotFound();
  }

  public async Task<IActionResult> BestPartners(int? year, int count = 10)
  {
    year ??= DateTime.Now.Year;

    var partners = await ctx.BestPartners(year.Value, count).ToListAsync();

    IDocument pdfDocument = new BestPartnersPdf(partners, $"Top {count} best partners in year {year}");
    byte[] pdf = pdfDocument.GeneratePdf();

    if (pdf != null)
    {
      Response.Headers.Append("content-disposition", "inline; filename=dokumenti.pdf");
      return File(pdf, "application/pdf");
    }
    else
    {
      return NotFound();
    }
  }

  #region Private methods  
  #region Header and rows for countries
  private void AddColumnsForCountries(TableDescriptor table)
  {
    table.ColumnsDefinition(columns =>
    {
      columns.ConstantColumn(25);
      columns.RelativeColumn(2);
      columns.RelativeColumn(3);
      columns.RelativeColumn();      
    });

    table.Header(header =>
    {
      header.Cell().CommonHeaderCellStyle().AlignRight().Text("#");
      header.Cell().CommonHeaderCellStyle().AlignLeft().PaddingLeft(5).Text("Country Code");
      header.Cell().CommonHeaderCellStyle().AlignLeft().Text("Country Name");
      header.Cell().CommonHeaderCellStyle().AlignCenter().Text("ISO3");      
    });
  }

  private void AddCountry(TableDescriptor table, int i, Country country, Func<IContainer, IContainer> cellStyle)
  {
    table.Cell().Element(cellStyle).AlignRight().Text($"{i}.");
    table.Cell().Element(cellStyle).AlignLeft().PaddingLeft(5).Text(country.CountryCode);
    table.Cell().Element(cellStyle).AlignLeft().Text(country.CountryName);
    table.Cell().Element(cellStyle).AlignCenter().Text(country.CountryIso3);    
  }

  #endregion

  #region Headers for products
  private void AddColumnsForProducts(TableDescriptor table)
  {
    table.ColumnsDefinition(columns =>
    {
      columns.ConstantColumn(25);
      columns.ConstantColumn(1.2f, Unit.Inch);
      columns.RelativeColumn(1);
      columns.RelativeColumn(3);
      columns.RelativeColumn();
    });

    table.Header(header =>
    {
      header.Cell().CommonHeaderCellStyle().AlignRight().Text("#");
      header.Cell().CommonHeaderCellStyle().AlignCenter().Text("");
      header.Cell().CommonHeaderCellStyle().AlignLeft().PaddingLeft(5).Text("Number");
      header.Cell().CommonHeaderCellStyle().AlignLeft().Text("Name");
      header.Cell().CommonHeaderCellStyle().AlignCenter().Text("Price");
    });
    #endregion
  }
  #endregion
}
