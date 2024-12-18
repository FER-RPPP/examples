﻿using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MVC_EN.ViewModels.PDFs;

public class DocumentsPdf : MasterDetailPdf<Order>
{
  private readonly IEnumerable<Order> documents;
  private readonly IUrlHelper urlHelper;

  public DocumentsPdf(IEnumerable<Order> documents, string naslov, IUrlHelper urlHelper) : base(documents, naslov)
  {
    this.documents = documents;
    this.urlHelper = urlHelper;
  }

  public override void RenderMaster(Order order, IContainer container)
  {    
    container.Row(row =>
    {
      row.RelativeItem().Column(column =>
      {
        column.Spacing(5);
        column.Item().Text(text =>
        {
          text.Span("Date: ").Bold();
          text.Span($"{order.DocumentDate:dd.MM.yyyy.}");
        });

        column.Item().Text(text =>
        {
          text.Span("Partner: ").Bold();
          text.Span($"{order.VatNumber} - {order.PartnerName}");
        });

        column.Item().Text(text =>
        {
          text.Span("Amount: ").Bold();
          text.Span($"{order.Amount:C2}");
        });
      });

      row.ConstantItem(100).AlignRight().Hyperlink(urlHelper.Action("Show", "Dokument", new { id = order.DocumentId })).Text($"#{order.DocumentId}");
    });
  }

  public override void RenderDetails(Order order, IContainer container)
  {
    container.PaddingTop(5).Table(table =>
    {               
      table.ColumnsDefinition(columns =>
      {
        columns.ConstantColumn(25);
        columns.RelativeColumn(4);
        columns.RelativeColumn(2);
        columns.RelativeColumn(2);
        columns.RelativeColumn(2);
        columns.RelativeColumn(2);
      });

      table.Header(header =>
      {
        header.Cell().Element(TableHeaderStyle).Text("#");
        header.Cell().Element(TableHeaderStyle).AlignCenter().Text("Product Name");
        header.Cell().Element(TableHeaderStyle).AlignRight().Text("Quantity");
        header.Cell().Element(TableHeaderStyle).AlignRight().Text("Unit Price");
        header.Cell().Element(TableHeaderStyle).AlignRight().Text("Discount");
        header.Cell().Element(TableHeaderStyle).AlignRight().Text("Total");       
      });

      int pos = 0;  
      foreach (var stavka in order.Items)
      {
        ++pos;

        table.Cell().Element(CellStyle).Text($"{pos}.");
        table.Cell().Element(CellStyle).Text(stavka.ProductName);
        table.Cell().Element(CellStyle).AlignRight().Text(stavka.Quantity.ToString("N2"));
        table.Cell().Element(CellStyle).AlignRight().Text(stavka.UnitPrice.ToString("C2"));
        table.Cell().Element(CellStyle).AlignRight().Text(stavka.Discount.ToString("P2"));
        table.Cell().Element(CellStyle).AlignRight().Text(stavka.ItemPrice.ToString("C2"));

        IContainer CellStyle(IContainer container)
        {
          return container.Background(pos % 2 == 0 ? Colors.White : Colors.Grey.Lighten4).PaddingVertical(5).DefaultTextStyle(x => x.FontSize(10));
        }       
      }
      table.Cell().ColumnSpan(5).AlignRight().Text("Total without VAT: ").Bold();
      table.Cell().PaddingRight(5).Text(order.Items.Sum(a => a.ItemPrice).ToString("N2")).AlignRight().Bold();
    });
  }

  
}