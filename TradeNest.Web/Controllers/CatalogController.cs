using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeNest.Data;
using TradeNest.Web.ViewModels;

namespace TradeNest.Web.Controllers;

public class CatalogController : Controller
{
    private TradeNestDbContext _dbContext;

    public CatalogController(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        CategoriesAndProductsViewModel categoriesAndProductsViewModel
            = new CategoriesAndProductsViewModel()
            {
                Products = this._dbContext.Products
                    .AsNoTracking()
                    .OrderByDescending(p => p.CreatedOn)
                    .Select(p => new ProductViewModel()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        SellingPrice = p.SellingPrice.ToString("F2"),
                        FrontImageUrl = p.Images
                            .FirstOrDefault(i => i.IsFrontImage == true)!
                            .Url,
                        CategoryName = p.Category.Name,
                    }),
                AllCategories = this._dbContext.Categories
                    .AsNoTracking()
                    .OrderBy(c => c.Name)
                    .Select(c => new CategoryViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
            };
        
        return View(categoriesAndProductsViewModel);
    }
}
