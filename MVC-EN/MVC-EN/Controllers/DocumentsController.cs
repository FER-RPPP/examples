using Microsoft.AspNetCore.Mvc;
using MVC_EN.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MVC_EN.ViewModels;
using MVC_EN.Extensions;
using MVC_EN.Extensions.Selectors;

namespace MVC_EN.Controllers;

public class DocumentsController : Controller
{
  private readonly FirmContext ctx;
  private readonly AppSettings appData;

  public DocumentsController(FirmContext ctx, IOptionsSnapshot<AppSettings> options)
  {
    this.ctx = ctx;
    appData = options.Value;
  }

  public async Task<IActionResult> Index(string filter, int page = 1, int sort = 1, bool ascending = true)
  {
    int pagesize = appData.PageSize;
    var query = ctx.vw_Documents.AsQueryable();

    #region Apply filter
    DocumentFilter df = DocumentFilter.FromString(filter);
    if (!df.IsEmpty())
    {
      if (df.PartnerId.HasValue)
      {
        df.PartnerName = await ctx.vw_Partners
                                  .Where(p => p.PartnerId == df.PartnerId)
                                  .Select(p => p.PartnerName)
                                  .FirstAsync();
      }
      query = df.Apply(query);
    }
    #endregion

    int count = await query.CountAsync();
  
    var pagingInfo = new PagingInfo
    {
      CurrentPage = page,
      Sort = sort,
      Ascending = ascending,
      ItemsPerPage = pagesize,
      TotalItems = count
    };
    
    if (count > 0 && (page < 1 || page > pagingInfo.TotalPages))
    {        
      return RedirectToAction(nameof(Index), new { page = 1, sort, ascending, filter });                
    }

    query = query.ApplySort(sort, ascending);

    var model = await DocumentsViewModel.CreateAsync(query, pagingInfo, df);

    for (int i = 0; i < model.Data.Count; i++)
    {
      model.Data[i].Position = (page - 1) * pagesize + i;
    }

    return View(model);
  }



  [HttpPost]
  public IActionResult Filter(DocumentFilter filter)
  {
    return RedirectToAction(nameof(Index), new { filter = filter.ToString() });
  }

  public async Task<IActionResult> Show(int id, int? position, string filter, int page = 1, int sort = 1, bool ascending = true, string viewName = nameof(Show))
  {      
    var document = await ctx.Documents
                            .Where(d => d.DocumentId == id)
                            .Select(d => new DocumentViewModel
                            {
                              DocumentNo = d.DocumentNo,
                              DocumentDate = d.DocumentDate,
                              DocumentId = d.DocumentId,
                              PartnerId = d.PartnerId,
                              PreviousDocumentId = d.PreviousDocumentId,
                              Amount = d.Amount,
                              VAT = d.Vat,
                              DocumentType = d.DocumentType
                            })
                            .FirstOrDefaultAsync();
    if (document == null)
    {
      return NotFound($"Document {id} does not exist.");
    }
    else
    {        
      document.PartnerName = await ctx.vw_Partners
                                      .Where(p => p.PartnerId == document.PartnerId)
                                      .Select(p => p.PartnerName)
                                      .FirstAsync();

      if (document.PreviousDocumentId.HasValue)
      {
        document.PreviousDocumentName = await ctx.vw_Documents                                           
                                                   .Where(d => d.DocumentId == document.PreviousDocumentId)
                                                   .Select(d => d.DocumentId + " " + d.PartnerName + " " + d.Amount)
                                                   .FirstOrDefaultAsync();
      }
      //loading items
      var items = await ctx.Items
                           .Where(s => s.DocumentId == document.DocumentId)
                           .OrderBy(s => s.ItemId)
                           .Select(s => new ItemViewModel
                           {
                              ItemId = s.ItemId,
                              UnitPrice = s.UnitPrice,
                              Quantity = s.Quantity,
                              ProductName = s.ProductNumberNavigation.ProductName,
                              Discount = s.Discount,
                              ProductNumber = s.ProductNumber
                           })
                           .ToListAsync();
      document.Items = items;

      if (position.HasValue)
      {
        page = 1 + position.Value / appData.PageSize;
        await SetPreviousAndNext(position.Value, filter, sort, ascending);
      }

      ViewBag.Page = page;
      ViewBag.Sort = sort;
      ViewBag.Ascending = ascending;
      ViewBag.Filter = filter;
      ViewBag.Position = position;

      return View(viewName, document);
    }
  }   

  private async Task SetPreviousAndNext(int position, string filter, int sort, bool ascending)
  {
    var query = ctx.vw_Documents.AsQueryable();                     

    DocumentFilter df = new DocumentFilter();
    if (!string.IsNullOrWhiteSpace(filter))
    {
      df = DocumentFilter.FromString(filter);
      if (!df.IsEmpty())
      {
        query = df.Apply(query);
      }
    }

    query = query.ApplySort(sort, ascending);      
    if (position > 0)
    {
      ViewBag.Previous = await query.Skip(position - 1).Select(d => d.DocumentId).FirstAsync();
    }
    if (position < await query.CountAsync() - 1) 
    {
      ViewBag.Next = await query.Skip(position + 1).Select(d => d.DocumentId).FirstAsync();
    }
  }

  [HttpGet]
  public async Task<IActionResult> Create()
  {
    int maxNo = await ctx.Documents.MaxAsync(d => d.DocumentNo) + 1; //just for example, in real scenarios this can cause problems due to concurent access
    var document = new DocumentViewModel
    {
      DocumentDate = DateTime.Now,
      DocumentNo = maxNo
    };
    return View(document);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(DocumentViewModel model)
  {
    if (ModelState.IsValid)
    {
      Document d = new Document();
      d.DocumentNo = model.DocumentNo;
      d.DocumentDate = model.DocumentDate.Date;
      d.PartnerId = model.PartnerId!.Value;
      d.PreviousDocumentId = model.PreviousDocumentId;
      d.Vat = model.VAT;
      d.DocumentType = model.DocumentType!;
      foreach (var itemFromModel in model.Items)
      {
        Item item = new Item();
        item.ProductNumber = itemFromModel.ProductNumber;
        item.Quantity = itemFromModel.Quantity;
        item.Discount = itemFromModel.Discount;
        item.UnitPrice = itemFromModel.UnitPrice;
        d.Items.Add(item);
      }

      d.Amount = (1 + d.Vat) * d.Items.Sum(i => i.ItemPrice);
      // a bussiness logic should be to apply additional discount, e.g. based on previous purchases etc.
      // and it would be additional reason not to develop the application as a fat client
      // but to use layering
      try
      {
        ctx.Add(d);
        await ctx.SaveChangesAsync();

        TempData[Constants.Message] = $"Document added. Id={d.DocumentId}";
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Edit), new { id = d.DocumentId });

      }
      catch (Exception exc)
      {
        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
        return View(model);
      }
    }
    else
    {
      return View(model);
    }
  }

  [HttpGet]
  public Task<IActionResult> Edit(int id, int? position, string filter, int page = 1, int sort = 1, bool ascending = true)
  {
    return Show(id, position, filter, page, sort, ascending, viewName: nameof(Edit));
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(DocumentViewModel model, int? position, string filter, int page = 1, int sort = 1, bool ascending = true)
  {
    ViewBag.Page = page;
    ViewBag.Sort = sort;
    ViewBag.Ascending = ascending;
    ViewBag.Filter = filter;
    ViewBag.Position = position;
    if (ModelState.IsValid)
    {
      var document = await ctx.Documents
                              .Include(d => d.Items)
                              .Where(d => d.DocumentId == model.DocumentId)
                              .FirstOrDefaultAsync();
      if (document == null)
      {
        return NotFound("There is no document with id: " + model.DocumentId);
      }

      if (position.HasValue)
      {
        page = 1 + position.Value / appData.PageSize;
        await SetPreviousAndNext(position.Value, filter, sort, ascending);
      }

      document.DocumentNo = model.DocumentNo;
      document.DocumentDate = model.DocumentDate.Date;
      document.PartnerId = model.PartnerId!.Value;
      document.PreviousDocumentId = model.PreviousDocumentId;
      document.Vat = model.VatAsInt / 100m;
      document.DocumentType = model.DocumentType!;

      List<int> itemsIds = model.Items
                                .Where(s => s.ItemId > 0)
                                .Select(s => s.ItemId)
                                .ToList();
      //remove all items that are not anymore in the model
      ctx.RemoveRange(document.Items.Where(i => !itemsIds.Contains(i.ItemId)));

      foreach (var item in model.Items)
      {
        //update current ones and add new
        Item newOrUpdatedItem; 
        if (item.ItemId > 0)
        {
          newOrUpdatedItem = document.Items.First(s => s.ItemId == item.ItemId);
        }
        else
        {
          newOrUpdatedItem = new ();
          document.Items.Add(newOrUpdatedItem);
        }
        newOrUpdatedItem.ProductNumber = item.ProductNumber;
        newOrUpdatedItem.Quantity = item.Quantity;
        newOrUpdatedItem.Discount = item.Discount;
        newOrUpdatedItem.UnitPrice = item.UnitPrice;
      }

      document.Amount = (1 + document.Vat) * model.Items.Sum(s => s.ItemPrice);
      try
      {
        await ctx.SaveChangesAsync();

        TempData[Constants.Message] = $"Document {document.DocumentId} updated.";
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Edit), new
        {
          id = document.DocumentId,
          position,
          filter,
          page,
          sort,
          ascending
        });

      }
      catch (Exception exc)
      {
        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
        return View(model);
      }
    }
    else
    {
      return View(model);
    }
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Delete(int id, string filter, int page = 1, int sort = 1, bool ascending = true)
  {
    var document = await ctx.Documents                             
                            .Where(d => d.DocumentId == id)
                            .SingleOrDefaultAsync();
    if (document != null)
    {
      try
      {
        ctx.Remove(document);
        await ctx.SaveChangesAsync();
        TempData[Constants.Message] = $"Document {document.DocumentId} deleted.";
        TempData[Constants.ErrorOccurred] = false;
      }
      catch (Exception exc)
      {
        TempData[Constants.Message] = $"Error deleting document {document.DocumentId} {exc.CompleteExceptionMessage()}";
        TempData[Constants.ErrorOccurred] = true;
      }
    }
    else
    {
      TempData[Constants.Message] = "Invalid document id: " + id;
      TempData[Constants.ErrorOccurred] = true;
    }
    return RedirectToAction(nameof(Index), new { filter, page, sort, ascending });
  }
}
