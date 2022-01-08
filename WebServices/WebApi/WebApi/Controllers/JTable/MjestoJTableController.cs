using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi.Models;
using WebApi.Models.JTable;

namespace WebApi.Controllers.JTable
{
  [Route("jtable/mjesto/[action]")]  
  public class MjestoJTableController : JTableController<MjestoController, int, MjestoViewModel>
  {
    public MjestoJTableController(MjestoController controller) : base(controller)
    {

    }

    [HttpPost]
    public async Task<JTableAjaxResult> Update([FromForm] MjestoViewModel model)
    {
      return await base.UpdateItem(model.IdMjesta, model);
    }

    [HttpPost]
    public async Task<JTableAjaxResult> Delete([FromForm] int IdMjesta)
    {
      return await base.DeleteItem(IdMjesta);
    }
  }
}