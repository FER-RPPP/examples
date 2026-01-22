using Microsoft.AspNetCore.Mvc;
using WebApi.Models.JTable;
using Contract.DTOs;

namespace WebApi.Controllers.JTable;

[Route("jtable/cities/[action]")]  
public class CitiesJTableController : JTableController<CitiesController, int, City>
{
  public CitiesJTableController(CitiesController controller) : base(controller)
  {

  }

  [HttpPost]
  public async Task<JTableAjaxResult> Update([FromForm] City model)
  {
    return await base.UpdateItem(model.CityId, model);
  }

  [HttpPost]
  public async Task<JTableAjaxResult> Delete([FromForm] int CityId)
  {
    return await base.DeleteItem(CityId);
  }
}