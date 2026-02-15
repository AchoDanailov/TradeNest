using TradeNest.Web.ViewModels.Category;
using TradeNest.Services.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TradeNest.Web.Controllers;

[Authorize]
public class CategoriesController : BaseController
{
    private readonly ICategoriesService _categoriesService;

    public CategoriesController(ICategoriesService categoriesService)
    {
        this._categoriesService = categoriesService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        IEnumerable<AllCategoriesWithMostSoldProductFrontImageViewModel> model
            = await this._categoriesService.GetAllCategoriesWithBestSellerImageVm();
        
        return View(model);
    }
}