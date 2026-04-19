using Microsoft.AspNetCore.Mvc;

namespace TradeNest.Web.Areas.MyNest.Controllers;

public class HomeController : BaseMyNestController
{
    public IActionResult Index()
    {
        return View();
    }
}