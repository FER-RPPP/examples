using MVC.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MVC.ViewModels.PDFs;

public class NajboljiPartneriPdf : AbstractPdf
{
  private readonly IEnumerable<NajboljiPartner> data;

  public NajboljiPartneriPdf(IEnumerable<NajboljiPartner> data, string title) : base(title) 
  {
    this.data = data;
  }

  public static TextStyle TitleStyle => TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

  public override void Compose(IDocumentContainer container)
  {
    container.Page(page =>
    {
      page.Margin(50);
      page.Header().AlignCenter().DefaultTextStyle(TitleStyle).Text(title);
      page.Footer().Element(ComposeFooter);

      page.Content().PaddingTop(5).Table(table =>
      {
        table.ColumnsDefinition(columns =>
        {
          columns.ConstantColumn(25);
          columns.ConstantColumn(110);
          columns.RelativeColumn(5);
          columns.RelativeColumn(2);         
        });

        table.Header(header =>
        {
          header.Cell().Element(TableHeaderStyle).AlignCenter().Text("#");
          header.Cell().Element(TableHeaderStyle).AlignCenter().PaddingLeft(10).Text("OIB");
          header.Cell().Element(TableHeaderStyle).AlignLeft().Text("Naziv partnera");
          header.Cell().Element(TableHeaderStyle).AlignCenter().Text("Iznos");
        });

        foreach (var (item, i) in data.Select((item, i) => (item, i + 1)))
        {
          var paran = i % 2 == 0;
          IContainer CellStyle(IContainer container)
          {
            return container.Background(paran ? Colors.White : Colors.Grey.Lighten4).PaddingVertical(5);
          }
          table.Cell().Element(CellStyle).AlignRight().Text($"{i}.");
          table.Cell().Element(CellStyle).AlignLeft().PaddingLeft(10).Text(item.OIB);
          table.Cell().Element(CellStyle).AlignLeft().Text(item.Naziv);
          table.Cell().Element(CellStyle).AlignRight().Text(item.Iznos.ToString("N2"));
        }

        table.Cell().ColumnSpan(3).AlignRight().Text("Ukupno: ").Bold();
        table.Cell().Text(data.Sum(a => a.Iznos).ToString("N2")).AlignRight().Bold();
      });
    });   
  }  
}
