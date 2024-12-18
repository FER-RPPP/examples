using MVC_EN.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MVC_EN.ViewModels.PDFs;

public class BestPartnersPdf : AbstractPdf
{
  private readonly IEnumerable<BestPartner> data;

  public BestPartnersPdf(IEnumerable<BestPartner> data, string title) : base(title) 
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
          header.Cell().Element(TableHeaderStyle).AlignCenter().PaddingLeft(10).Text("VAT Number");
          header.Cell().Element(TableHeaderStyle).AlignLeft().Text("Partner Name");
          header.Cell().Element(TableHeaderStyle).AlignCenter().Text("Amount");
        });

        foreach (var (item, i) in data.Select((item, i) => (item, i + 1)))
        {
          var paran = i % 2 == 0;
          IContainer CellStyle(IContainer container)
          {
            return container.Background(paran ? Colors.White : Colors.Grey.Lighten4).PaddingVertical(5);
          }
          table.Cell().Element(CellStyle).AlignRight().Text($"{i}.");
          table.Cell().Element(CellStyle).AlignLeft().PaddingLeft(10).Text(item.VatNumber);
          table.Cell().Element(CellStyle).AlignLeft().Text(item.PartnerName);
          table.Cell().Element(CellStyle).AlignRight().Text(item.Amount.ToString("N2"));
        }

        table.Cell().ColumnSpan(3).AlignRight().Text("Total: ").Bold();
        table.Cell().Text(data.Sum(a => a.Amount).ToString("N2")).AlignRight().Bold();
      });
    });   
  }  
}
