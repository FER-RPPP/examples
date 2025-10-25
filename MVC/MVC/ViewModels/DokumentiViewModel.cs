using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.ViewModels;

public class DokumentiViewModel(List<ViewDokumentInfo> data, PagingInfo pagingInfo, DokumentFilter filter) : PagedList<ViewDokumentInfo>(data, pagingInfo)
{  
  public DokumentFilter Filter { get; set; } = filter;

  public static async Task<PagedList<ViewDokumentInfo>> CreateAsync(IQueryable<ViewDokumentInfo> source, PagingInfo pagingInfo, DokumentFilter filter, CancellationToken cancellationToken = default)
  {
    var items = await source
                        .Skip((pagingInfo.CurrentPage - 1) * pagingInfo.ItemsPerPage)
                        .Take(pagingInfo.ItemsPerPage)
                        .ToListAsync(cancellationToken);
    return new DokumentiViewModel(items, pagingInfo, filter);
  }
}
