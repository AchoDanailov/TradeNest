using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TradeNest.Web.Controllers;

[AllowAnonymous]
public class HomeController : BaseController
{
    [HttpGet]
    [Route("/")] [Route("/Home")] [Route("/Home/Index")]
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
