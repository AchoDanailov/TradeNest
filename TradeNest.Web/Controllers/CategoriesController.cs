using TradeNest.Data;
using TradeNest.Web.ViewModels.Category;
using TradeNest.GCommon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Web.Controllers;

[Authorize]
public class CategoriesController : BaseController
{
    private TradeNestDbContext _dbContext;

    public CategoriesController(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        IEnumerable<AllCategoriesWithMostSoldProductFrontImageViewModel> categoriesViewModels 
            = this._dbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new AllCategoriesWithMostSoldProductFrontImageViewModel()
                {
                    Id = c.Id,
                    CategoryName = c.Name,
                    MostSoldProductFrontImageUrl = c.Products.Any() 
                        ? c.Products
                            .OrderBy(p => p.ProductsOrders.Count)
                            .First()
                            .Images.Any()
                            ? c.Products
                                .OrderBy(p => p.ProductsOrders.Count)
                                .First()
                                .Images.First(i => i.IsFrontImage).Url
                            : ApplicationConstants.DefaultProductImageUrl
                        : ApplicationConstants.DefaultProductImageUrl
                })
                .ToArray();

        return View(categoriesViewModels);
    }
}