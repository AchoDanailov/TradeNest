using Microsoft.AspNetCore.Mvc;

namespace TradeNest.Web.Areas.Admin.Controllers;

public class ProductsManagementController : BaseAdminController
{
    public IActionResult Index()
    {
        return View(new
        {
            ReturnUrl = Url.Action(nameof(Index), controller: "Home", new { area = "Admin" })
        });
    }
}