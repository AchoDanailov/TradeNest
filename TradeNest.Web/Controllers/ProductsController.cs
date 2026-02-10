using TradeNest.Data;
using TradeNest.Data.Models;
using TradeNest.Web.ViewModels;
using static TradeNest.GCommon.ApplicationConstants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Web.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private TradeNestDbContext _dbContext;
    private IEnumerable<string> _categoriesNames;
        
    public ProductsController(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
        this._categoriesNames = this.GetCategoriesNames();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index(
        [FromQuery] string? category = null,
        [FromQuery] string? search = null)
    {
        CatalogProductsAndCategoriesViewModel viewModel
            = new CatalogProductsAndCategoriesViewModel()
            {
                CategoriesNames = this._categoriesNames,
            };
        
        IQueryable<Product> products = this._dbContext.Products
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            viewModel.IsSearchResultSet = true;
            products = products
                .Where(p => p.Name.Contains(search));
        }

        bool isValidCategory = viewModel.CategoriesNames
            .Any(categoryName => categoryName == category);
        if (string.IsNullOrEmpty(category) || isValidCategory)
        {
            return NotFound();
        }
        
        products = products
            .Include(p => p.Category)
            .Where(p => p.Category.Name == category);

        IEnumerable<ProductViewModel> productsViewModels = products
            .OrderByDescending(p => p.CreatedOn)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice.ToString(PricesFormat),
                FrontImageUrl = p.Images
                    .SingleOrDefault(i => i.IsFrontImage == true)!
                    .Url,
                CategoryName = p.Category.Name,
            })
            .ToArray();

        viewModel.Products = productsViewModels;
        
        return View(viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult BestSellers()
    {
        CatalogProductsAndCategoriesViewModel viewModel
            = new CatalogProductsAndCategoriesViewModel()
            {
                CategoriesNames = this._categoriesNames,
                IsSearchResultSet = false,
            };
        
        IEnumerable<ProductViewModel> products = this._dbContext.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedOn)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice.ToString(PricesFormat),
                FrontImageUrl = p.Images
                    .SingleOrDefault(i => i.IsFrontImage == true)!
                    .Url,
                CategoryName = p.Category.Name,
            })
            .ToArray();

        viewModel.Products = products;
        
        return View(nameof(Index), viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Details([FromRoute] string? id = null)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guidIdValue))
        {
            return BadRequest();
        }

        Product? product = this._dbContext.Products
            .Include(p => p.Owner)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsNoTracking()
            .FirstOrDefault(p => p.Id == guidIdValue);
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
            SellingPrice = product.SellingPrice.ToString(PricesFormat),
            IsEnabled = product.IsEnabled,
            Owner = product.Owner?.UserName ?? string.Empty,
            CategoryName = product.Category.Name,
            FrontImageUrl = product.Images
                .SingleOrDefault(i => i.IsFrontImage)!
                .Url,
            ImagesUrls = product.Images
                .Select(i => i.Url),
        };
        
        return View(productDetailsViewModel);
    }

    private IEnumerable<string> GetCategoriesNames()
    {
        return this._dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => c.Name)
            .ToArray();
    }
}