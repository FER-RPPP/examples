
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using MVC_EN.Extensions;
using MVC_EN.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MVC_EN.Controllers;

public class ReportsController : Controller
{
  private readonly FirmContext ctx;
  private readonly IWebHostEnvironment environment;
  private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

  public ReportsController(FirmContext ctx, IWebHostEnvironment environment)
  {
    this.ctx = ctx;
    this.environment = environment;
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
    string title = "Countries";
    var countries = await ctx.Countries
                             .AsNoTracking()
                             .OrderBy(d => d.CountryName)
                             .ToListAsync();
    PdfReport report = CreateReport(title);
    #region Header and footer
    report.PagesFooter(footer =>
    {
      footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
    })
    .PagesHeader(header =>
    {
      header.CacheHeader(cache: true); // It's a default setting to improve the performance.
            header.DefaultHeader(defaultHeader =>
      {
        defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
        defaultHeader.Message(title);
      });
    });
    #endregion
    #region Set datasource and define columns
    report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(countries));

    report.MainTableColumns(columns =>
    {
      columns.AddColumn(column =>
      {
        column.IsRowNumber(true);
        column.CellsHorizontalAlignment(HorizontalAlignment.Right);
        column.IsVisible(true);
        column.Order(0);
        column.Width(1);
        column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
      });

      columns.AddColumn(column =>
      {
        column.PropertyName(nameof(Country.CountryCode));
        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
        column.IsVisible(true);
        column.Order(1);
        column.Width(2);
        column.HeaderCell("Country Code");
      });

      columns.AddColumn(column =>
      {
        column.PropertyName<Country>(x => x.CountryName);
        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
        column.IsVisible(true);
        column.Order(2);
        column.Width(3);
        column.HeaderCell("Country Name", horizontalAlignment: HorizontalAlignment.Center);
      });

      columns.AddColumn(column =>
      {
        column.PropertyName<Country>(x => x.CountryIso3);
        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
        column.IsVisible(true);
        column.Order(3);
        column.Width(1);
        column.HeaderCell("ISO3", horizontalAlignment: HorizontalAlignment.Center);
      });
    });

    #endregion
    byte[] pdf = report.GenerateAsByteArray();

    if (pdf != null)
    {
      Response.Headers.Add("content-disposition", "inline; filename=countries.pdf");
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
    PdfReport report = CreateReport(title);
    #region Header and footer
    report.PagesFooter(footer =>
    {
      footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
    })
    .PagesHeader(header =>
    {
      header.CacheHeader(cache: true); // It's a default setting to improve the performance.
            header.DefaultHeader(defaultHeader =>
      {
        defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
        defaultHeader.Message(title);
      });
    });
    #endregion
    #region Set data source and define columns
    report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(products));

    report.MainTableSummarySettings(summarySettings =>
    {
      summarySettings.OverallSummarySettings("Total");
    });

    report.MainTableColumns(columns =>
    {
      columns.AddColumn(column =>
      {
        column.IsRowNumber(true);
        column.CellsHorizontalAlignment(HorizontalAlignment.Right);
        column.IsVisible(true);
        column.Order(0);
        column.Width(1);
        column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
      });

      columns.AddColumn(column =>
      {
        column.PropertyName(nameof(Product.Photo));
        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
        column.IsVisible(true);
        column.Order(1);
        column.Width(1);
        column.HeaderCell(" ");
        column.ColumnItemsTemplate(t => t.ByteArrayImage(string.Empty, fitImages: true));
      });

      columns.AddColumn(column =>
      {
        column.PropertyName(nameof(Product.ProductNumber));
        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
        column.IsVisible(true);
        column.Order(2);
        column.Width(1);
        column.HeaderCell("Number");
      });

      columns.AddColumn(column =>
      {
        column.PropertyName<Product>(x => x.ProductName);
        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
        column.IsVisible(true);
        column.Order(3);
        column.Width(4);
        column.HeaderCell("Name", horizontalAlignment: HorizontalAlignment.Center);
      });

      columns.AddColumn(column =>
      {
        column.PropertyName<Product>(x => x.Price);
        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
        column.IsVisible(true);
        column.Order(4);
        column.Width(1);
        column.HeaderCell("Price", horizontalAlignment: HorizontalAlignment.Center);
        column.ColumnItemsTemplate(template =>
        {
          template.TextBlock();
          template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                        ? string.Empty : string.Format("{0:C2}", obj));
        });
        column.AggregateFunction(aggregateFunction =>
        {
          aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
          aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                        ? string.Empty : string.Format("{0:C2}", obj));
        });
      });
    });

    #endregion
    byte[] pdf = report.GenerateAsByteArray();

    if (pdf != null)
    {
      Response.Headers.Add("content-disposition", "inline; filename=products.pdf");
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
    var param = new SqlParameter("N", n); //SQL Parameter from Microsoft.Data.SqlClient, and not from System.Data.SqlClient
    string title = $"Top {n} biggets purchaces";
    var items = await ctx.BiggestPurchases(n)                            
                         .OrderBy(s => s.DocumentId)
                         .ThenBy(s => s.ProductName)
                         .ToListAsync();
    items.ForEach(s => s.DocumentUrl = Url.Action("Edit", "Documents", new { id = s.DocumentId }));
    PdfReport report = CreateReport(title);
    #region Header and footer
    report.PagesFooter(footer =>
    {
      footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
    })
    .PagesHeader(header =>
    {
      header.CacheHeader(cache: true); // It's a default setting to improve the performance.
            header.CustomHeader(new MasterDetailsHeaders(title)
      {
        PdfRptFont = header.PdfFont
      });
    });
    #endregion
    #region Set datasource and define columns
    report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(items));

    report.MainTableSummarySettings(summarySettings =>
    {
      summarySettings.OverallSummarySettings("Total");
    });

    report.MainTableColumns(columns =>
    {
      #region Columns used for groupings
      columns.AddColumn(column =>
      {
        column.PropertyName<ItemDenorm>(s => s.DocumentId);
        column.Group(
            (val1, val2) =>
            {
              return (int)val1 == (int)val2;
            });
      });
      #endregion
      columns.AddColumn(column =>
      {
        column.IsRowNumber(true);
        column.CellsHorizontalAlignment(HorizontalAlignment.Right);
        column.IsVisible(true);
        column.Width(1);
        column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
      });
      columns.AddColumn(column =>
      {
        column.PropertyName<ItemDenorm>(x => x.ProductName);
        column.CellsHorizontalAlignment(HorizontalAlignment.Center);
        column.IsVisible(true);
        column.Width(4);
        column.HeaderCell("Product Name", horizontalAlignment: HorizontalAlignment.Center);
      });

      columns.AddColumn(column =>
      {
        column.PropertyName<ItemDenorm>(x => x.Quantity);
        column.CellsHorizontalAlignment(HorizontalAlignment.Right);
        column.IsVisible(true);
        column.Width(1);
        column.HeaderCell("Quantity", horizontalAlignment: HorizontalAlignment.Center);
        column.ColumnItemsTemplate(template =>
        {
          template.TextBlock();
          template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                        ? string.Empty : string.Format("{0:.00}", obj));
        });
      });

      columns.AddColumn(column =>
      {
        column.PropertyName<ItemDenorm>(x => x.UnitPrice);
        column.CellsHorizontalAlignment(HorizontalAlignment.Right);
        column.IsVisible(true);
        column.Width(1);
        column.HeaderCell("Unit price", horizontalAlignment: HorizontalAlignment.Center);
        column.ColumnItemsTemplate(template =>
        {
          template.TextBlock();
          template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                        ? string.Empty : string.Format("{0:C2}", obj));
        });
      });

      columns.AddColumn(column =>
      {
        column.PropertyName<ItemDenorm>(x => x.Discount);
        column.CellsHorizontalAlignment(HorizontalAlignment.Right);
        column.IsVisible(true);
        column.Width(1);
        column.HeaderCell("Discount", horizontalAlignment: HorizontalAlignment.Center);
        column.ColumnItemsTemplate(template =>
        {
          template.TextBlock();
          template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                        ? string.Empty : string.Format("{0:P2}", obj));
        });
      });

      columns.AddColumn(column =>
      {
        column.PropertyName("Total");
        column.CellsHorizontalAlignment(HorizontalAlignment.Right);
        column.IsVisible(true);
        column.Width(1);
        column.HeaderCell("Total", horizontalAlignment: HorizontalAlignment.Center);
        column.ColumnItemsTemplate(template =>
        {
          template.TextBlock();
          template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                        ? string.Empty : string.Format("{0:C2}", obj));
        });
        column.AggregateFunction(aggregateFunction =>
        {
          aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
          aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                        ? string.Empty : string.Format("{0:C2}", obj));
        });
        column.CalculatedField(
                list =>
                {
                  if (list == null) return string.Empty;
                  decimal quantity = (decimal)list.GetValueOf(nameof(ItemDenorm.Quantity));
                  decimal discount = (decimal)list.GetValueOf(nameof(ItemDenorm.Discount));
                  decimal unitPrice = (decimal)list.GetValueOf(nameof(ItemDenorm.UnitPrice));
                  var iznos = unitPrice * quantity * (1 - discount);
                  return iznos;
                });
      });
    });

    #endregion
    byte[] pdf = report.GenerateAsByteArray();

    if (pdf != null)
    {
      Response.Headers.Add("content-disposition", "inline; filename=documents.pdf");
      return File(pdf, "application/pdf");
    }
    else
      return NotFound();
  }

  #region Master-detail header
  public class MasterDetailsHeaders : IPageHeader
  {
    private string title;
    public MasterDetailsHeaders(string title)
    {
      this.title = title;
    }
    public IPdfFont PdfRptFont { set; get; }

    public PdfGrid RenderingGroupHeader(iTextSharp.text.Document pdfDoc, PdfWriter pdfWriter, IList<CellData> newGroupInfo, IList<SummaryCellData> summaryData)
    {
      var documentId = newGroupInfo.GetSafeStringValueOf(nameof(ItemDenorm.DocumentId));
      var documentUrl = newGroupInfo.GetSafeStringValueOf(nameof(ItemDenorm.DocumentUrl));
      var partnerName = newGroupInfo.GetSafeStringValueOf(nameof(ItemDenorm.PartnerName));
      var documentDate = (DateTime)newGroupInfo.GetValueOf(nameof(ItemDenorm.DocumentDate));
      var documentAmount = (decimal)newGroupInfo.GetValueOf(nameof(ItemDenorm.DocumentAmount));

      var table = new PdfGrid(relativeWidths: new[] { 2f, 5f, 2f, 3f }) { WidthPercentage = 100 };

      table.AddSimpleRow(
          (cellData, cellProperties) =>
          {
            cellData.Value = "Document Id:";
            cellProperties.PdfFont = PdfRptFont;
            cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
            cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
          },
          (cellData, cellProperties) =>
          {
            cellData.TableRowData = newGroupInfo; //postavi podatke retka za ćeliju
                    var cellTemplate = new HyperlinkField(BaseColor.Black, false)
            {
              TextPropertyName = nameof(ItemDenorm.DocumentId),
              NavigationUrlPropertyName = nameof(ItemDenorm.DocumentUrl),
              BasicProperties = new CellBasicProperties
              {
                HorizontalAlignment = HorizontalAlignment.Left,
                PdfFontStyle = DocumentFontStyle.Bold,
                PdfFont = PdfRptFont
              }
            };

            cellData.CellTemplate = cellTemplate;
            cellProperties.PdfFont = PdfRptFont;
            cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
          },
          (cellData, cellProperties) =>
          {
            cellData.Value = "Document date:";
            cellProperties.PdfFont = PdfRptFont;
            cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
            cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
          },
          (cellData, cellProperties) =>
          {
            cellData.Value = documentDate;
            cellProperties.PdfFont = PdfRptFont;
            cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
            cellProperties.DisplayFormatFormula = obj => ((DateTime)obj).ToString("dd.MM.yyyy");
          });

      table.AddSimpleRow(
          (cellData, cellProperties) =>
          {
            cellData.Value = "Partner:";
            cellProperties.PdfFont = PdfRptFont;
            cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
            cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
          },
          (cellData, cellProperties) =>
          {
            cellData.Value = partnerName;
            cellProperties.PdfFont = PdfRptFont;
            cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
          },
          (cellData, cellProperties) =>
          {
            cellData.Value = "Amount:";
            cellProperties.PdfFont = PdfRptFont;
            cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
            cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
          },
          (cellData, cellProperties) =>
          {
            cellData.Value = documentAmount;
            cellProperties.DisplayFormatFormula = obj => ((decimal)obj).ToString("C2");
            cellProperties.PdfFont = PdfRptFont;
            cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
          });
      return table.AddBorderToTable(borderColor: BaseColor.LightGray, spacingBefore: 5f);
    }

    public PdfGrid RenderingReportHeader(iTextSharp.text.Document pdfDoc, PdfWriter pdfWriter, IList<SummaryCellData> summaryData)
    {
      var table = new PdfGrid(numColumns: 1) { WidthPercentage = 100 };
      table.AddSimpleRow(
         (cellData, cellProperties) =>
         {
           cellData.Value = title;
           cellProperties.PdfFont = PdfRptFont;
           cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
           cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
         });
      return table.AddBorderToTable();
    }
  }
  #endregion

  #region Private methods
  private PdfReport CreateReport(string title)
  {
    var pdf = new PdfReport();

    pdf.DocumentPreferences(doc =>
    {
      doc.Orientation(PageOrientation.Portrait);
      doc.PageSize(PdfPageSize.A4);
      doc.DocumentMetadata(new DocumentMetadata
      {
        Author = "FER-ZPR",
        Application = "Firma.MVC Core",
        Title = title
      });
      doc.Compression(new CompressionSettings
      {
        EnableCompression = true,
        EnableFullCompression = true
      });
    })
    //fix za linux https://github.com/VahidN/PdfReport.Core/issues/40
    .DefaultFonts(fonts => {
      fonts.Path(Path.Combine(environment.WebRootPath, "fonts", "verdana.ttf"),
                       Path.Combine(environment.WebRootPath, "fonts", "tahoma.ttf"));
      fonts.Size(9);
      fonts.Color(System.Drawing.Color.Black);
    })
    //
    .MainTableTemplate(template =>
    {
      template.BasicTemplate(BasicTemplate.ProfessionalTemplate);
    })
    .MainTablePreferences(table =>
    {
      table.ColumnsWidthsType(TableColumnWidthType.Relative);
            //table.NumberOfDataRowsPerPage(20);
            table.GroupsPreferences(new GroupsPreferences
      {
        GroupType = GroupType.HideGroupingColumns,
        RepeatHeaderRowPerGroup = true,
        ShowOneGroupPerPage = true,
        SpacingBeforeAllGroupsSummary = 5f,
        NewGroupAvailableSpacingThreshold = 150,
        SpacingAfterAllGroupsSummary = 5f
      });
      table.SpacingAfter(4f);
    });

    return pdf;
  }
  #endregion
}
