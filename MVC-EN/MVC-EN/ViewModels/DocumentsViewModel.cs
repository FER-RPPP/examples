using Microsoft.EntityFrameworkCore;
using MVC_EN.Models;

namespace MVC_EN.ViewModels;

public class DocumentsViewModel(List<ViewDocument> data, PagingInfo pagingInfo, DocumentFilter filter) : PagedList<ViewDocument>(data, pagingInfo)
{
  public DocumentFilter Filter { get; set; } = filter;
  public static async Task<PagedList<ViewDocument>> CreateAsync(IQueryable<ViewDocument> source, PagingInfo pagingInfo, DocumentFilter filter, CancellationToken cancellationToken = default)
  {
    var items = await source
                        .Skip((pagingInfo.CurrentPage - 1) * pagingInfo.ItemsPerPage)
                        .Take(pagingInfo.ItemsPerPage)
                        .ToListAsync(cancellationToken);
    return new DocumentsViewModel(items, pagingInfo, filter);
  }
}
