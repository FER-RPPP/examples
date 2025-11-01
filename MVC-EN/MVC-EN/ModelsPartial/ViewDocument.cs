using MVC_EN.Util;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_EN.Models;

public class ViewDocument
{
  public int DocumentId { get; set; }

  [ExcelFormat("0.00%")]
  public decimal VAT { get; set; }
  public int? PreviousDocumentId { get; set; }

  [Display(Name = "Date")]
  [ExcelFormat("dd.mm.yyyy")]
  public DateTime DocumentDate { get; set; }
  public int PartnerId { get; set; }
  public required string PartnerName { get; set; }

  [ExcelFormat("#,###,##0.00")]
  public decimal Amount { get; set; }
  public required string DocumentType { get; set; }
  public int DocumentNo { get; set; }

  [NotMapped]    
  public int Position { get; set; } //Position in the result
}
