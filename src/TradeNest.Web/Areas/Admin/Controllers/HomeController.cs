using Microsoft.AspNetCore.Mvc;

namespace TradeNest.Web.Areas.Admin.Controllers;

public class HomeController : BaseAdminController 
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    } 
}