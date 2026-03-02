using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TradeNest.Web.Controllers;

[AllowAnonymous]
public class HomeController : BaseController
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        return View();
    }
}
