using QuestPDF.Elements.Table;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MVC.Extensions;

public static class PdfExtensions
{
  public static void SetCommonData(this PageDescriptor page, string title)
  {
    page.Size(PageSizes.A4);
    page.Margin(1, Unit.Centimetre);
    page.DefaultTextStyle(x => x.FontSize(10));

    page.Header().AlignCenter().Text(title).Bold().FontSize(16);

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
  }

  public static IDocument CreateTableReport<T>(this IEnumerable<T> data, 
                                   string title,
                                   Action<TableDescriptor> createHeader,
                                   Action<TableDescriptor, int, T, Func<IContainer, IContainer>> createRow,
                                   Action<TableDescriptor> createFooter = null
                                  )
  {
    var report = Document.Create(container =>
    {          
      container.Page(page =>
      {
        page.SetCommonData(title);
        
        page.Content().Table(table =>
        {
          createHeader(table);

          foreach (var (item, i) in data.Select((item, i) => (item, i + 1)))
          {
            var isEven = i % 2 == 0;
            IContainer CellStyle(IContainer container)
            {
              return container.Background(isEven ? Colors.White : Colors.Grey.Lighten4).PaddingVertical(5);
            }
            createRow(table, i, item, CellStyle);
          }

          createFooter?.Invoke(table);
        });
      });
    });

    report.WithMetadata(new DocumentMetadata
    {
      Author = "FER-ZPR",
      Title = title,
    });

    return report;
  }

  public static IContainer CommonHeaderCellStyle(this ITableCellContainer cell) => cell.Element(CommonTableHeaderCellStyle);

  private static IContainer CommonTableHeaderCellStyle(IContainer container) 
    => container.DefaultTextStyle(x => x.SemiBold())
                .PaddingVertical(5)
                .BorderBottom(1)
                .BorderColor(Colors.Black);
}