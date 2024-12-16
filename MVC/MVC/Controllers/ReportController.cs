using MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using MVC.Extensions;
using MVC.ViewModels.PDFs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MVC.Controllers;

public class ReportController : Controller
{
  private readonly FirmaContext ctx;
  private readonly IWebHostEnvironment environment;
  private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

  public ReportController(FirmaContext ctx, IWebHostEnvironment environment)
  {
    this.ctx = ctx;
    this.environment = environment;
  }

  public IActionResult Index()
  {
    return View();
  }

  #region Export u Excel
  public async Task<IActionResult> DrzaveExcel()
  {
    var drzave = await ctx.Drzava
                          .AsNoTracking()
                          .OrderBy(d => d.NazDrzave)
                          .ToListAsync();
    byte[] content;
    using (ExcelPackage excel = new ExcelPackage())
    {
      excel.Workbook.Properties.Title = "Popis država";
      excel.Workbook.Properties.Author = "FER-ZPR";
      var worksheet = excel.Workbook.Worksheets.Add("Države");

      //First add the headers
      worksheet.Cells[1, 1].Value = "Oznaka države";
      worksheet.Cells[1, 2].Value = "Naziv države";
      worksheet.Cells[1, 3].Value = "ISO3";
      worksheet.Cells[1, 4].Value = "Šifra države";

      for (int i = 0; i < drzave.Count; i++)
      {
        worksheet.Cells[i + 2, 1].Value = drzave[i].OznDrzave;
        worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        worksheet.Cells[i + 2, 2].Value = drzave[i].NazDrzave;
        worksheet.Cells[i + 2, 3].Value = drzave[i].Iso3drzave;
        worksheet.Cells[i + 2, 4].Value = drzave[i].SifDrzave;
      }

      worksheet.Cells[1, 1, drzave.Count + 1, 4].AutoFitColumns();

      content = excel.GetAsByteArray();
    }
    return File(content, ExcelContentType, "drzave.xlsx");
  }

  public async Task<IActionResult> DokumentiExcel()
  {
    var dokumenti = await ctx.vw_Dokumenti
                              .OrderByDescending(d => d.IznosDokumenta)
                              .Take(10)
                              .ToListAsync();
    byte[] content;
    using (ExcelPackage excel = dokumenti.CreateExcel("Dokumenti"))
    {
      content = excel.GetAsByteArray();
    }
    return File(content, ExcelContentType, "dokumenti.xlsx");
  }

  #endregion

  public async Task<IActionResult> Drzave()
  {
    var drzave = await ctx.Drzava
                          .AsNoTracking()
                          .OrderBy(d => d.NazDrzave)
                          .ToListAsync();

    var report = CreateReport("Popis država", drzave, DodajZaglavljeZaDrzave, DodajDrzavu);

    byte[] pdf = report.GeneratePdf();

    if (pdf != null)
    {
      Response.Headers.Append("content-disposition", "inline; filename=drzave.pdf");
      return File(pdf, "application/pdf");
      //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
    }
    else
    {
      return NotFound();
    }
  }

  public async Task<IActionResult> Artikli()
  {
    var artikli = await ctx.Artikl
                            .Where(a => a.SlikaArtikla != null)
                            .OrderByDescending(a => a.CijArtikla)
                            .Select(a => new
                            {
                              a.SifArtikla,
                              a.NazArtikla,
                              a.CijArtikla,
                              a.SlikaArtikla
                            })
                            .Take(10)
                            .ToListAsync();


    var report = CreateReport("Deset najskupljih artikala koji imaju sliku", artikli, DodajZaglavljeZaArtikle,
      (table, i, artikl, cellStyle) =>
      {
        table.Cell().Element(cellStyle).AlignRight().Text($"{i}.");
        table.Cell().Element(cellStyle).AlignCenter().MaxHeight(0.9f, Unit.Inch).MaxWidth(1, Unit.Inch).Image(artikl.SlikaArtikla);
        table.Cell().Element(cellStyle).AlignLeft().PaddingLeft(5).Text(artikl.SifArtikla.ToString());
        table.Cell().Element(cellStyle).AlignCenter().Text(artikl.NazArtikla);
        table.Cell().Element(cellStyle).AlignRight().PaddingRight(5).Text(artikl.CijArtikla.ToString("N2"));
      },
      table =>
      {
        table.Cell().ColumnSpan(4).AlignRight().Text("UKUPNO: ").Bold();
        table.Cell().PaddingRight(5).Text(artikli.Sum(a => a.CijArtikla).ToString("N2")).AlignRight().Bold();
      }
    );

    byte[] pdf = report.GeneratePdf();

    if (pdf != null)
    {
      Response.Headers.Append("content-disposition", "inline; filename=artikli.pdf");
      return File(pdf, "application/pdf");
    }
    else
    {
      return NotFound();
    }
  }

  public async Task<IActionResult> Dokumenti()
  {
    int n = 10;
    var partneri = ctx.vw_Partner;
    var dokumenti = await ctx.Dokument
                             .Join(partneri, d => d.IdPartnera, p => p.IdPartnera, (d, p) => new Kupnja
                             {
                               DatDokumenta = d.DatDokumenta,
                               IdDokumenta = d.IdDokumenta,
                               IznosDokumenta = d.IznosDokumenta,
                               NazPartnera = p.Naziv,
                               Stavke = d.Stavka.Select(s => new StavkaKupnje
                               {
                                 IdStavke = s.IdStavke,
                                 JedCijArtikla = s.JedCijArtikla,
                                 KolArtikla = s.KolArtikla,
                                 NazArtikla = s.SifArtiklaNavigation.NazArtikla,
                                 PostoRabat = s.PostoRabat,
                                 SifArtikla = s.SifArtikla
                               })
                             })
                             .OrderByDescending(d => d.IznosDokumenta)
                             .Take(n)
                             .ToListAsync();

    IDocument pdfDocument = new DokumentiPdf(dokumenti, $"{n} najvećih kupnji", Url);
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

  public async Task<IActionResult> NajboljiPartneri(int? godina, int broj = 10)
  {
    godina ??= DateTime.Now.Year;

    var partneri = await ctx.NajboljiPartneri(godina.Value, broj).ToListAsync();

    IDocument pdfDocument = new NajboljiPartneriPdf(partneri, $"{broj} najboljih partnera u {godina}. godini");
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

  private Document CreateReport<T>(string naslov,
                                   IEnumerable<T> podaci,
                                   Action<TableDescriptor> dodajZaglavlje,
                                   Action<TableDescriptor, int, T, Func<IContainer, IContainer>> dodajRedak,
                                   Action<TableDescriptor> dodajPodnozje = null
                                  )
  {
    var report = Document.Create(container =>
    {
      container.Page(page =>
      {
        page.Size(PageSizes.A4);
        page.Margin(1, Unit.Centimetre);
        page.DefaultTextStyle(x => x.FontSize(10));

        page.Header().AlignCenter().Text(naslov).Bold().FontSize(16);

        page.Footer().Inlined(inlined =>
        {
          inlined.AlignJustify();

          inlined.Item()
                .Text(DateTime.Now.ToString("dd.MM.yyyy."));

          inlined.Item()
                 .Text(x =>
                 {
                   x.CurrentPageNumber();
                   x.Span(" / ");
                   x.TotalPages();
                 });

        });

        page.Content().Table(table =>
        {
          dodajZaglavlje(table);

          foreach (var (podatak, i) in podaci.Select((podatak, i) => (podatak, i + 1)))
          {
            var paran = i % 2 == 0;
            IContainer CellStyle(IContainer container)
            {
              return container.Background(paran ? Colors.White : Colors.Grey.Lighten4).PaddingVertical(5);
            }
            dodajRedak(table, i, podatak, CellStyle);
          }

          dodajPodnozje?.Invoke(table);
        });
      });
    });

    report.WithMetadata(new DocumentMetadata
    {
      Author = "FER-ZPR",
      Title = naslov,
    });

    return report;
  }

  static IContainer HeaderCellStyle(IContainer container)
  {
    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
  }

  #region Zaglavlje i redak za države
  private void DodajZaglavljeZaDrzave(TableDescriptor table)
  {
    table.ColumnsDefinition(columns =>
    {
      columns.ConstantColumn(25);
      columns.RelativeColumn(2);
      columns.RelativeColumn(3);
      columns.RelativeColumn();
      columns.RelativeColumn();
    });

    table.Header(header =>
    {
      header.Cell().Element(HeaderCellStyle).AlignRight().Text("#");
      header.Cell().Element(HeaderCellStyle).AlignLeft().PaddingLeft(5).Text("Oznaka države");
      header.Cell().Element(HeaderCellStyle).AlignLeft().Text("Naziv države");
      header.Cell().Element(HeaderCellStyle).AlignCenter().Text("ISO3");
      header.Cell().Element(HeaderCellStyle).AlignCenter().Text("Šifra države");
    });
  }
  #endregion

  private void DodajDrzavu(TableDescriptor table, int i, Drzava drzava, Func<IContainer, IContainer> cellStyle)
  {
    table.Cell().Element(cellStyle).AlignRight().Text($"{i}.");
    table.Cell().Element(cellStyle).AlignLeft().PaddingLeft(5).Text(drzava.OznDrzave);
    table.Cell().Element(cellStyle).AlignLeft().Text(drzava.NazDrzave);
    table.Cell().Element(cellStyle).AlignCenter().Text(drzava.Iso3drzave);
    table.Cell().Element(cellStyle).AlignCenter().Text(drzava.SifDrzave.ToString());
  }

  #region Zaglavlje za artikle
  private void DodajZaglavljeZaArtikle(TableDescriptor table)
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
      header.Cell().Element(HeaderCellStyle).AlignRight().Text("#");
      header.Cell().Element(HeaderCellStyle).AlignCenter().Text("");
      header.Cell().Element(HeaderCellStyle).AlignLeft().PaddingLeft(5).Text("Šifra");
      header.Cell().Element(HeaderCellStyle).AlignLeft().Text("Naziv artikla");
      header.Cell().Element(HeaderCellStyle).AlignCenter().Text("Cijena");
    });
    #endregion
  }
  #endregion
}