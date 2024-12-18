using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MVC.ViewModels.PDFs;

public abstract class AbstractPdf : IDocument
{
  protected readonly string title;

  protected AbstractPdf(string title)
  {
    this.title = title;
  }

  public virtual DocumentMetadata GetMetadata() => new DocumentMetadata
  {
    Title = title,
    Author = "FER-ZPR"
  };

  protected static IContainer TableHeaderStyle(IContainer container)
  {
    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
  }

  public abstract void Compose(IDocumentContainer container);

  protected virtual void ComposeFooter(IContainer container)
  {
    container.DefaultTextStyle(x => x.FontSize(10))
    .Inlined(inlined =>
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
}
