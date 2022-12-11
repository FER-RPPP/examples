using MVC_EN.Models;

namespace MVC_EN.ViewModels;

public class DocumentsViewModel
  {
      public IEnumerable<ViewDocument> Documents { get; set; }
      public PagingInfo PagingInfo { get; set; }
      public DocumentFilter Filter { get; set; }
  }
