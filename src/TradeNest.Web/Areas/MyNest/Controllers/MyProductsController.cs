using Microsoft.AspNetCore.Mvc;

namespace TradeNest.Web.Areas.MyNest.Controllers;

public class MyProductsController : BaseMyNestController
{
    public IActionResult Index()
    {
        return View();
    } 
}