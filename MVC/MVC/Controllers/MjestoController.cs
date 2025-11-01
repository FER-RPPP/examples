using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MVC.Extensions;
using MVC.Extensions.Selectors;
using MVC.Models;
using MVC.ViewModels;
using System.Linq.Expressions;
using System.Text.Json;

namespace MVC.Controllers;

public class MjestoController : Controller
{
  private readonly FirmaContext ctx;
  private readonly AppSettings appData;

  private static Expression<Func<Mjesto, MjestoViewModel>> selectProjection = m => new MjestoViewModel
  {
    IdMjesta = m.IdMjesta,
    NazivMjesta = m.NazMjesta,
    PostBrojMjesta = m.PostBrMjesta,
    PostNazivMjesta = m.PostNazMjesta,
    OznDrzave = m.OznDrzave,
    NazivDrzave = m.OznDrzaveNavigation.NazDrzave
  };

  public MjestoController(FirmaContext ctx, IOptionsSnapshot<AppSettings> options)
  {
    this.ctx = ctx;
    appData = options.Value;
  }

  public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
  {
    int pagesize = appData.PageSize;

    var query = ctx.Mjesto.AsNoTracking();
    int count = await query.CountAsync();

    var pagingInfo = new PagingInfo
    {
      CurrentPage = page,
      Sort = sort,
      Ascending = ascending,
      ItemsPerPage = pagesize,
      TotalItems = count
    };
    if (page < 1 || page > pagingInfo.TotalPages)
    {
      return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
    }

    query = query.ApplySort(sort, ascending);

    var mjesta = query.Select(selectProjection);
    var model = await PagedList<MjestoViewModel>.CreateAsync(mjesta, pagingInfo);

    return View(model);
  }   

  [HttpGet]
  public async Task<IActionResult> Create()
  {
    await PrepareDropDownLists();
    return View();
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(MjestoViewModel model)
  {
    if (ModelState.IsValid)
    {
      try
      {
        var mjesto = new Mjesto
        {
          OznDrzave = model.OznDrzave,
          NazMjesta = model.NazivMjesta,
          PostBrMjesta = model.PostBrojMjesta,
          PostNazMjesta = model.PostNazivMjesta
        };
        ctx.Add(mjesto);
        await ctx.SaveChangesAsync();

        TempData[Constants.Message] = $"Mjesto {model.NazivMjesta} dodano. Id mjesta = {mjesto.IdMjesta}";
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Index));

      }
      catch (Exception exc)
      {
        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
        await PrepareDropDownLists();
        return View(model);
      }
    }
    else
    {
      await PrepareDropDownLists();
      return View(model);
    }
  }

  private async Task PrepareDropDownLists()
  {
    var hr = await ctx.Drzava                  
                      .Where(d => d.OznDrzave == "HR")
                      .Select(d => new { d.NazDrzave, d.OznDrzave })
                      .FirstOrDefaultAsync();
    var drzave = await ctx.Drzava                      
                          .Where(d => d.OznDrzave != "HR")
                          .OrderBy(d => d.NazDrzave)
                          .Select(d => new { d.NazDrzave, d.OznDrzave })
                          .ToListAsync();
    if (hr != null)
    {
      drzave.Insert(0, hr);
    }      
    ViewBag.Drzave = new SelectList(drzave, nameof(hr.OznDrzave), nameof(hr.NazDrzave));
  }
  

  #region Metode za dinamičko brisanje i ažuriranje
  [HttpDelete]
  public async Task<IActionResult> Delete(int id)
  {
    ActionResponseMessage responseMessage;
    var mjesto = await ctx.Mjesto.FindAsync(id);          
    if (mjesto != null)
    {
      try
      {
        string naziv = mjesto.NazMjesta;
        ctx.Remove(mjesto);
        await ctx.SaveChangesAsync();
        responseMessage = new ActionResponseMessage(MessageType.Success, $"Mjesto {naziv} sa šifrom {id} uspješno obrisano.");          
      }
      catch (Exception exc)
      {          
        responseMessage = new ActionResponseMessage(MessageType.Error, $"Pogreška prilikom brisanja mjesta: {exc.CompleteExceptionMessage()}");
      }
    }
    else
    {
      responseMessage = new ActionResponseMessage(MessageType.Error, $"Mjesto sa šifrom {id} ne postoji");
    }

    Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
    return responseMessage.MessageType == MessageType.Success ?
      new EmptyResult() : await Get(id);
  }

  [HttpGet]
  public async Task<IActionResult> Edit(int id)
  {
    var mjesto = await ctx.Mjesto
                          .Where(m => m.IdMjesta == id)
                          .Select(selectProjection)
                          .SingleOrDefaultAsync();
    if (mjesto != null)
    {        
      await PrepareDropDownLists();
      return PartialView(mjesto);
    }
    else
    {
      return NotFound($"Neispravan id mjesta: {id}");
    }
  }

  [HttpPost]  
  public async Task<IActionResult> Edit(MjestoViewModel model)
  {
    if (model == null)
    {
      return NotFound("Nema poslanih podataka");
    }
   
    if (ModelState.IsValid)
    {
      try
      {
        var mjesto = await ctx.Mjesto.FindAsync(model.IdMjesta);
        if (mjesto == null)
        {
          return NotFound($"Neispravan id mjesta: {model?.IdMjesta}");
        }
        mjesto.NazMjesta = model.NazivMjesta;
        mjesto.PostBrMjesta = model.PostBrojMjesta; 
        mjesto.PostNazMjesta = model.PostNazivMjesta;
        mjesto.OznDrzave = model.OznDrzave; 

        await ctx.SaveChangesAsync();
        return RedirectToAction(nameof(Get), new { id = model.IdMjesta });
      }
      catch (Exception exc)
      {
        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
        await PrepareDropDownLists();
        return PartialView(model);
      }
    }
    else
    {
      await PrepareDropDownLists();
      return PartialView(model);
    }
  }

  [HttpGet]
  public async Task<IActionResult> Get(int id)
  {
    var mjesto = await ctx.Mjesto                            
                          .Where(m => m.IdMjesta == id)
                          .Select(selectProjection)
                          .SingleOrDefaultAsync();
    if (mjesto != null)
    {
      return PartialView(mjesto);
    }
    else
    {
      return NotFound($"Neispravan id mjesta: {id}");
    }
  }
  #endregion
}
