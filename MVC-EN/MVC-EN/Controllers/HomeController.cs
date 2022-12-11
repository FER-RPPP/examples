using Microsoft.AspNetCore.Mvc;

namespace MVC_EN.Controllers;

public class HomeController : Controller
{
  public IActionResult Index()
  {
    return View();
  }
}
