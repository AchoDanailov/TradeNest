using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeNest.Data;
using TradeNest.Web.ViewModels;

namespace TradeNest.Web.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private TradeNestDbContext _dbContext;

    public ProductsController(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult All(
        [FromQuery] string? category = null,
        [FromQuery] string? search = null, 
        [FromQuery] string? orderBy = null)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            return Json(search); 
        }
        
        if (!string.IsNullOrEmpty(category))
        {
            return Json(category);
        }
        
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            return Json(orderBy); 
        }
        
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

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Details([FromRoute] string? id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guidId))
        {
            return BadRequest();
        }

        var product = this._dbContext.Products
            .Include(p => p.Owner)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == guidId);
        if (product == null)
        {
            return NotFound();
        }

        ProductDetailsViewModel productDetailsViewModel = new ProductDetailsViewModel()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            QuantityInStock = product.QuantityInStock,
            SellingPrice = product.SellingPrice.ToString("f2"),
            IsEnabled = product.IsEnabled,
            Owner = product.Owner!.UserName ?? string.Empty,
            CategoryName = product.Category.Name,
            FrontImageUrl = product.Images
                .FirstOrDefault(i => i.IsFrontImage == true)!
                .Url,
            ImagesUrls = product.Images
                .Select(i => i.Url),
        };
        
        return View(productDetailsViewModel);
    }
}