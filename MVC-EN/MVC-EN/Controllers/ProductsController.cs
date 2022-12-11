using Microsoft.AspNetCore.Mvc;
using MVC_EN.Extensions;
using MVC_EN.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MVC_EN.ViewModels;
using MVC_EN.Extensions.Selectors;
using MVC_EN.Util;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace MVC_EN.Controllers;

public class ProductsController : Controller
{
  private readonly FirmContext ctx;
  private readonly ILogger<ProductsController> logger;
  private readonly AppSettings appData;

  public ProductsController(FirmContext ctx, IOptionsSnapshot<AppSettings> options, ILogger<ProductsController> logger)
  {
    this.ctx = ctx;
    this.logger = logger;
    appData = options.Value;
  }

  public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
  {
    int pagesize = appData.PageSize;
    var query = ctx.Products.AsNoTracking();
    int count = await query.CountAsync();

    var pagingInfo = new PagingInfo
    {
      CurrentPage = page,
      Sort = sort,
      Ascending = ascending,
      ItemsPerPage = pagesize,
      TotalItems = count
    };
    if (page < 1 || page > pagingInfo.TotalPages) {       
      return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
    }

    query = query.ApplySort(sort, ascending);      

    var product = await query
                        .Select(a => new ProductViewModel
                        {
                          ProductNumber = a.ProductNumber,
                          ProductName = a.ProductName,
                          UnitName = a.UnitName,
                          Price = a.Price,
                          IsService = a.IsService,
                          Description = a.Description,
                          HasPhoto = a.Photo != null,
                          ImageHash = a.PhotoChecksum
                        })
                        .Skip((page - 1) * pagesize)
                        .Take(pagesize)
                        .ToListAsync();
    var model = new ProductsViewModel
    {
      Products = product,
      PagingInfo = pagingInfo
    };

    return View(model);
  }

  [HttpGet]
  public IActionResult Create()
  {
    return View();
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(Product product, IFormFile photo)
  {
    //check if product number is unique
    bool exists = await ctx.Products.AnyAsync(a => a.ProductNumber == product.ProductNumber);
    if (exists)
    {
      ModelState.AddModelError(nameof(Product.ProductNumber), "A product with the same number already exists.");
    }
    if (ModelState.IsValid)
    {
      try
      {
        if (photo != null && photo.Length > 0)
        {
          
          using (MemoryStream stream = new MemoryStream())
          {
            await photo.CopyToAsync(stream);
            byte[] image = stream.ToArray();              
            product.Photo = ImageUtil.CreateThumbnail(image, maxheight: appData.ImageSettings.ThumbnailHeight);
          }            
        }
        ctx.Add(product);
        await ctx.SaveChangesAsync();

        TempData[Constants.Message] = $"Product  {product.ProductNumber} - {product.ProductName} dodan";
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Index));

      }
      catch (Exception exc)
      {
        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
        return View(product);
      }
    }
    else
    {
      return View(product);
    }
  }

  public async Task<IActionResult> GetImage(int id)
  {
    
    int? checksum = await ctx.Products
                             .Where(a => a.ProductNumber == id)
                             .Select(a => a.PhotoChecksum)
                             .SingleOrDefaultAsync();
    if (checksum == null)
    {
      return NotFound();
    }
    
    string responseETag = "\"" + checksum.Value + "\"";
    if (Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var requestETag) && requestETag == responseETag)
    {
      //return StatusCode((int)HttpStatusCode.NotModified);
      return StatusCode(StatusCodes.Status304NotModified);
    }

    byte[] image = await ctx.Products
                            .Where(a => a.ProductNumber == id)
                            .Select(a => a.Photo)
                            .SingleOrDefaultAsync();

    return File(image, "image/jpeg", lastModified: DateTime.Now, entityTag: new EntityTagHeaderValue(responseETag));
  }

  [HttpGet]
  public async Task<IActionResult> Edit(int id)
  {
    var product = await ctx.Products
                          .Where(a => a.ProductNumber == id)
                          .Select(a => new Product
                          {
                            Price = a.Price,
                            UnitName = a.UnitName,
                            ProductName = a.ProductName,
                            ProductNumber = a.ProductNumber,
                            Description = a.Description,
                            IsService = a.IsService
                          })
                          .FirstOrDefaultAsync();

    if (product != null)
    {
      return PartialView(product);
    }
    else
    {
      return NotFound($"Invalid product id: {id}");
    }
  }

  [HttpPost]   
  public async Task<IActionResult> Edit(Product product, IFormFile photo, bool deletePhoto)
  {
    if (product == null)
    {
      return NotFound("No data submitted!");
    }
    var entity = await ctx.Products.FindAsync(product.ProductNumber);
    if (entity == null)
    {
      return NotFound($"Invalid product id: {product.ProductNumber}");
    }
  
    if (ModelState.IsValid)
    {
      try
      {
        //ctx.Update(product) must not be used, as photo had not been included in the model
        entity.UnitName = product.UnitName;
        entity.ProductName = product.ProductName;
        entity.IsService = product.IsService;
        entity.Description = product.Description;
        entity.Price = product.Price;

        if (photo != null && photo.Length > 0)
        {
          using (MemoryStream stream = new MemoryStream())
          {
            photo.CopyTo(stream);
            byte[] image = stream.ToArray();
            image = ImageUtil.CreateThumbnail(image, maxheight: appData.ImageSettings.ThumbnailHeight);
            entity.Photo = image;              
          }
        }
        else if (deletePhoto)
        {
          entity.Photo = null;
        }

        await ctx.SaveChangesAsync();
        return RedirectToAction(nameof(Get), new { id = product.ProductNumber });
      }
      catch (Exception exc)
      {
        ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
        return PartialView(product);
      }
    }
    else
    {
      return PartialView(product);
    }
  }

  public async Task<IActionResult> Get(int id)
  {
    var product = await ctx.Products                            
                          .Where(a => a.ProductNumber == id)
                          .Select(a => new ProductViewModel
                          {
                            ProductNumber = a.ProductNumber,
                            ProductName = a.ProductName,
                            UnitName = a.UnitName,
                            Price = a.Price,
                            IsService = a.IsService,
                            Description = a.Description,
                            HasPhoto = a.Photo != null,
                            ImageHash = a.PhotoChecksum
                          })
                          .SingleOrDefaultAsync();
    if (product != null)
    {
      return PartialView(product);
    }
    else
    {
      return NotFound($"Invalid product id: {id}");
    }
  }
 
  [HttpDelete]
  public async Task<IActionResult> Delete(int id)
  {
    ActionResponseMessage responseMessage;
    var product = await ctx.Products.FindAsync(id);
    if (product != null)
    {
      try
      {
        string name = product.ProductName;
        ctx.Remove(product);
        await ctx.SaveChangesAsync();
        responseMessage = new ActionResponseMessage(MessageType.Success, $"Product {name} with number {id} has been deleted.");
      }
      catch (Exception exc)
      {
        responseMessage = new ActionResponseMessage(MessageType.Error, $"Error deleting product #{id}: {exc.CompleteExceptionMessage()}");
      }
    }
    else
    {
      responseMessage = new ActionResponseMessage(MessageType.Error, $"Product with number {id} does not exist.");        
    }

    Response.Headers["HX-Trigger"] = JsonSerializer.Serialize(new { showMessage = responseMessage });
    return responseMessage.MessageType == MessageType.Success ?
     new EmptyResult() : await Get(id);
  }

  public async Task<bool> CheckProductNumber(int ProductNumber)
  {
    bool exists = await ctx.Products.AnyAsync(a => a.ProductNumber == ProductNumber);
    return !exists;
  }
}
