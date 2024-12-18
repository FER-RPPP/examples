using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MVC.ViewModels.PDFs;

public abstract class MasterDetailPdf<T> : AbstractPdf
{
  private readonly IEnumerable<T> data;

  protected MasterDetailPdf(IEnumerable<T> data, string title) : base(title) 
  {
    this.data = data;
  }

  public virtual TextStyle TitleStyle => TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

  public override void Compose(IDocumentContainer container)
  {
    container.Page(page =>
    {
      page.Margin(50);
      page.Header().AlignCenter().DefaultTextStyle(TitleStyle).Text(title);
      page.Footer().Element(ComposeFooter);
    
      page.Content().Column(col =>
      {
        bool first = true;
        foreach (var item in data)
        {
          if (!first)
          {
            col.Item().PageBreak();
          }
          first = false;

          col.Item().Element(c => RenderMaster(item, c));
          col.Item().Element(c => RenderMasterDetailSeparator(c));
          col.Item().Element(c => RenderDetails(item, c));
        }
      });
    });
  }

  public abstract void RenderMaster(T item, IContainer container);
  public abstract void RenderDetails(T item, IContainer container);
  public virtual void RenderMasterDetailSeparator(IContainer container) { }
}
