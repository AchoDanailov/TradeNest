using Microsoft.AspNetCore.Mvc;

namespace TradeNest.Web.Areas.Admin.Controllers;

public class HomeController : BaseAdminController 
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    } 
}