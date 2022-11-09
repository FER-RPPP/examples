using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MVC.Extensions;
using MVC.Extensions.Selectors;
using MVC.Models;
using MVC.ViewModels;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MVC.Controllers
{
  public class MjestoController : Controller
  {
    private readonly FirmaContext ctx;
    private readonly AppSettings appData;
   
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
     
      var mjesta = await query
                          .Select(m => new MjestoViewModel
                          {
                            IdMjesta = m.IdMjesta,
                            NazivMjesta = m.NazMjesta,
                            PostBrojMjesta = m.PostBrMjesta,
                            PostNazivMjesta = m.PostNazMjesta,
                            NazivDrzave = m.OznDrzaveNavigation.NazDrzave
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();
      var model = new MjestaViewModel
      {
        Mjesta = mjesta,
        PagingInfo = pagingInfo
      };

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
    public async Task<IActionResult> Create(Mjesto mjesto)
    {
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Add(mjesto);
          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"Mjesto {mjesto.NazMjesta} dodano. Id mjesta = {mjesto.IdMjesta}";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));

        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          await PrepareDropDownLists();
          return View(mjesto);
        }
      }
      else
      {
        await PrepareDropDownLists();
        return View(mjesto);
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
      return new EmptyResult();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var mjesto = await ctx.Mjesto
                            .AsNoTracking()
                            .Where(m => m.IdMjesta == id)
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
    public async Task<IActionResult> Edit(Mjesto mjesto)
    {
      if (mjesto == null)
      {
        return NotFound("Nema poslanih podataka");
      }
      bool checkId = await ctx.Mjesto.AnyAsync(m => m.IdMjesta == mjesto.IdMjesta);
      if (!checkId)
      {
        return NotFound($"Neispravan id mjesta: {mjesto?.IdMjesta}");
      }

      if (ModelState.IsValid)
      {
        try
        {
          ctx.Update(mjesto);
          await ctx.SaveChangesAsync();
          return RedirectToAction(nameof(Get), new { id = mjesto.IdMjesta });
        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          await PrepareDropDownLists();
          return PartialView(mjesto);
        }
      }
      else
      {
        await PrepareDropDownLists();
        return PartialView(mjesto);
      }
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
      var mjesto = await ctx.Mjesto                            
                            .Where(m => m.IdMjesta == id)
                            .Select(m => new MjestoViewModel
                            {
                              IdMjesta = m.IdMjesta,
                              NazivMjesta = m.NazMjesta,
                              PostBrojMjesta = m.PostBrMjesta,
                              PostNazivMjesta = m.PostNazMjesta,
                              NazivDrzave = m.OznDrzaveNavigation.NazDrzave
                            })
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
}
