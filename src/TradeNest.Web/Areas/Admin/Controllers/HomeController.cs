using Microsoft.AspNetCore.Mvc;

namespace TradeNest.Web.Areas.Admin.Controllers;

public class HomeController : BaseAdminController 
{
    [HttpGet]
    [Route("/Admin/Home/Index")] [Route("/Admin/Home")] [Route("/Admin/ControlPanel")]
    public async Task<IActionResult> Index()
    {
        return View();
    } 
}