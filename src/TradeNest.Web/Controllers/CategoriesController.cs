using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Category;
using TradeNest.GCommon;
using TradeNest.Web.Models.Category;

namespace TradeNest.Web.Controllers;

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
        IEnumerable<CategoryWithBestSellerImageDto> categoriesDto
            = await this._categoriesService.GetAllCategoriesWithBestSellerImageAsync();

        IEnumerable<AllCategoriesWithBestSellerFrontImageViewModel> viewModel = categoriesDto
            .Select(c => new AllCategoriesWithBestSellerFrontImageViewModel()
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                BestSellerImageUrl = c.BestSellerImageUrl ?? 
                                     ApplicationConstants.DefaultProductImageUrl,
            });
        
        return View(viewModel);
    }
}