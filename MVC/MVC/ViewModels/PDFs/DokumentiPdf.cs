using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MVC.ViewModels.PDFs;

public class DokumentiPdf : MasterDetailPdf<Kupnja>
{  
  private readonly IUrlHelper urlHelper;

  public DokumentiPdf(IEnumerable<Kupnja> dokumenti, string naslov, IUrlHelper urlHelper) : base(dokumenti, naslov)
  {   
    this.urlHelper = urlHelper;
  }

  public override void RenderMaster(Kupnja item, IContainer container)
  {    
    container.Row(row =>
    {
      row.RelativeItem().Column(column =>
      {
        column.Spacing(5);
        column.Item().Text(text =>
        {
          text.Span("Datum: ").Bold();
          text.Span($"{item.DatDokumenta:dd.MM.yyyy.}");
        });

        column.Item().Text(text =>
        {
          text.Span("Partner: ").Bold();
          text.Span($"{item.OIB} - {item.NazPartnera}");
        });

        column.Item().Text(text =>
        {
          text.Span("Iznos: ").Bold();
          text.Span($"{item.IznosDokumenta:C2}");
        });
      });

      row.ConstantItem(100).AlignRight().Hyperlink(urlHelper.Action("Show", "Dokument", new { id = item.IdDokumenta })).Text($"#{item.IdDokumenta}");
    });
  }

  public override void RenderDetails(Kupnja item, IContainer container)
  {
    container.PaddingTop(5).Table(table =>
    {               
      table.ColumnsDefinition(columns =>
      {
        columns.ConstantColumn(25);
        columns.RelativeColumn(4);
        columns.RelativeColumn();
        columns.RelativeColumn(2);
        columns.RelativeColumn();
        columns.RelativeColumn(2);
      });

      table.Header(header =>
      {
        header.Cell().Element(TableHeaderStyle).Text("#");
        header.Cell().Element(TableHeaderStyle).AlignCenter().Text("Naziv artikla");
        header.Cell().Element(TableHeaderStyle).AlignRight().Text("Količina");
        header.Cell().Element(TableHeaderStyle).AlignRight().Text("Jedinična cijena");
        header.Cell().Element(TableHeaderStyle).AlignRight().Text("Rabat");
        header.Cell().Element(TableHeaderStyle).AlignRight().Text("Ukupno");       
      });

      int pos = 0;  
      foreach (var stavka in item.Stavke)
      {
        ++pos;

        table.Cell().Element(CellStyle).Text($"{pos}.");
        table.Cell().Element(CellStyle).Text(stavka.NazArtikla);
        table.Cell().Element(CellStyle).AlignRight().Text(stavka.KolArtikla.ToString("N2"));
        table.Cell().Element(CellStyle).AlignRight().Text(stavka.JedCijArtikla.ToString("C2"));
        table.Cell().Element(CellStyle).AlignRight().Text(stavka.PostoRabat.ToString("P2"));
        table.Cell().Element(CellStyle).AlignRight().Text(stavka.CijenaStavke.ToString("C2"));

        IContainer CellStyle(IContainer container)
        {
          return container.Background(pos % 2 == 0 ? Colors.White : Colors.Grey.Lighten4).PaddingVertical(5).DefaultTextStyle(x => x.FontSize(10));
        }       
      }
      table.Cell().ColumnSpan(5).AlignRight().Text("Ukupno bez PDV-a: ").Bold();
      table.Cell().PaddingRight(5).Text(item.Stavke.Sum(a => a.CijenaStavke).ToString("N2")).AlignRight().Bold();
    });
  }

  
}