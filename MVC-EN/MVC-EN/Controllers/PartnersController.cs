using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MVC_EN.Extensions;
using MVC_EN.Extensions.Selectors;
using MVC_EN.Models;
using MVC_EN.ViewModels;
using System.Text.Json;

namespace MVC_EN.Controllers;

public class PartnersController : Controller
{
  private readonly FirmContext ctx;
  private readonly ILogger<PartnersController> logger;
  private readonly AppSettings appData;

  public PartnersController(FirmContext ctx, IOptionsSnapshot<AppSettings> options, ILogger<PartnersController> logger)
  {
    this.ctx = ctx;
    this.logger = logger;
    appData = options.Value;
  }

  public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
  {
    int pagesize = appData.PageSize;

    IQueryable<ViewPartner> query = ctx.vw_Partners.AsQueryable();
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
      return RedirectToAction(nameof(Index), new { page = 1, sort = sort, ascending = ascending });
    }

    query = query.ApplySort(sort, ascending);

    var partners = await query
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)                          
                        .ToListAsync();
    var model = await PagedList<ViewPartner>.CreateAsync(query, pagingInfo);

    return View(model);
  }

  [HttpGet]
  public IActionResult Create()
  {
    PartnerViewModel model = new PartnerViewModel
    {
      PartnerType = "P"
    };
    return View(model);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(PartnerViewModel model)
  {
    if (ModelState.IsValid)
    {
      Partner p = new Partner();
      p.PartnerType = model.PartnerType!;
      CopyValues(p, model);
      try
      {
        ctx.Add(p);
        await ctx.SaveChangesAsync();

        TempData[Constants.Message] = $"Partner added. Id={p.PartnerId}";
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Index));

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

  [HttpDelete]
  public async Task<IActionResult> Delete(int id)
  {
    ActionResponseMessage responseMessage;
    var partner = await ctx.Partners.FindAsync(id);
    if (partner != null)
    {
      try
      {
        ctx.Remove(partner);
        await ctx.SaveChangesAsync();
        responseMessage = new ActionResponseMessage(MessageType.Success, $"Partner {partner.PartnerId} deleted.");        
      }
      catch (Exception exc)
      {
        responseMessage = new ActionResponseMessage(MessageType.Error, $"Error deleting partner {partner.PartnerId}: {exc.CompleteExceptionMessage()}");          
      }
    }
    else
    {
      responseMessage = new ActionResponseMessage(MessageType.Error, "Invalid partner id: " + id);
    }

    Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
    return responseMessage.MessageType == MessageType.Success ?
      new EmptyResult() : await Get(id);
  }

  [HttpGet]
  public async Task<IActionResult> Edit(int id, int page = 1, int sort = 1, bool ascending = true)
  {
    var partner = await ctx.Partners
                           .Where(p => p.PartnerId == id)
                           .Select(p => new PartnerViewModel
                            {
                              PartnerId = p.PartnerId,
                              ShipmentCityId = p.ShipmentCityId,
                              ShipmentCityName = p.ShipmentCity.PostalCode + " " + p.ShipmentCity.CityName,
                              ResidenceCityId = p.ResidenceCityId,
                              ResidenceCityName = p.ResidenceCity.PostalCode + " " + p.ResidenceCity.CityName,
                              ShipmentAddress = p.ShipmentAddress,
                              ResidenceAddress = p.ResidenceAddress,
                              VATNumber = p.VatNumber,
                              PartnerType = p.PartnerType
                            })
                           .FirstOrDefaultAsync();
    if (partner == null)
    {
      return NotFound("Invalid partner id: " + id);
    }
    else
    {
      if (partner.PartnerType == "P")
      {
        Person person = ctx.People.AsNoTracking()
                                  .Where(o => o.PersonId == partner.PartnerId)
                                  .First(); //Single()
        partner.PersonFirstName = person.FirstName;
        partner.PersonLastName = person.LastName;
      }
      else
      {
        Company company = ctx.Companies.AsNoTracking()
                              .Where(t => t.CompanyId == partner.PartnerId)
                              .First(); //Single()
        partner.RegistrationNumber = company.RegistrationNumber;
        partner.CompanyName = company.CompanyName;
      }

      ViewBag.Page = page;
      ViewBag.Sort = sort;
      ViewBag.Ascending = ascending;
      return View(partner);
    }
  }



  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(PartnerViewModel model, int page = 1, int sort = 1, bool ascending = true)
  {
    if (model == null)
    {
      return NotFound("No data submitted?!");
    }

    var partner = await ctx.Partners.FindAsync(model.PartnerId);
    if (partner == null)
    {
      return NotFound("Invalid partner id: " + model.PartnerId);
    }

    ViewBag.Page = page;
    ViewBag.Sort = sort;
    ViewBag.Ascending = ascending;

    if (ModelState.IsValid)
    {
      try
      {
        CopyValues(partner, model);

        //Person or Company are created using new X, which means they would be treated as new entities, and INSERT would be generated, but we need update
        if (partner.Person != null)
        {
          partner.Person.PersonId = partner.PartnerId;
          ctx.Entry(partner.Person).State = EntityState.Modified;
        }
        if (partner.Company != null)
        {
          partner.Company.CompanyId = partner.PartnerId;
          ctx.Entry(partner.Company).State = EntityState.Modified;
        }

        await ctx.SaveChangesAsync();
        TempData[Constants.Message] = $"Partner {model.PartnerId} added";
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Index), new { page, sort, ascending });

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
  public async Task<IActionResult> Get(int id)
  {
    var partner = await ctx.vw_Partners                          
                          .Where(p => p.PartnerId == id)                          
                          .SingleOrDefaultAsync();
    if (partner != null)
    {
      return PartialView(partner);
    }
    else
    {
      return NotFound($"Invalid partner id: {id}");
    }
  }


  #region Private methods
  private void CopyValues(Partner partner, PartnerViewModel model)
  {
    partner.ShipmentAddress = model.ShipmentAddress;
    partner.ResidenceAddress = model.ResidenceAddress;
    partner.ShipmentCityId = model.ShipmentCityId;
    partner.ResidenceCityId = model.ResidenceCityId;
    partner.VatNumber = model.VATNumber;
    if (partner.PartnerType == "P")
    {
      partner.Person = new Person();
      partner.Person.FirstName = model.PersonFirstName!;
      partner.Person.LastName = model.PersonLastName!;
    }
    else
    {
      partner.Company = new Company();
      partner.Company.RegistrationNumber = model.RegistrationNumber!;
      partner.Company.CompanyName = model.CompanyName!;
    }
  }
  #endregion
}
