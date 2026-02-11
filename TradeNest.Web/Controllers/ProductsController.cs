using System.Security.Claims;
using TradeNest.Data;
using TradeNest.Data.Models;
using static TradeNest.GCommon.ApplicationConstants;
using static TradeNest.Web.Utilities.ErrorMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Product;

namespace TradeNest.Web.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private TradeNestDbContext _dbContext;
    private ILogger<ProductsController> _logger;
    
    public ProductsController(TradeNestDbContext dbContext, ILogger<ProductsController> logger)
    {
        this._dbContext = dbContext;
        this._logger = logger;
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
                CategoriesNames = this.GetCategoriesNames()
            };
        
        IQueryable<Product> products = this._dbContext.Products
            .AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            viewModel.IsSearchResultSet = true;
            products = products
                .Where(p => p.Name.Contains(search));
        }

        if (!string.IsNullOrEmpty(category))
        {
            bool isValidCategory = viewModel.CategoriesNames
                .Any(categoryName => categoryName == category);
            if (!isValidCategory)
            {
                return NotFound();
            }
        
            products = products
                .Include(p => p.Category)
                .Where(p => p.Category.Name == category);
        }

        IEnumerable<ProductViewModel> productsViewModels = products
            .OrderByDescending(p => p.CreatedOn)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice.ToString(PricesFormat),
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .SingleOrDefault(i => i.IsFrontImage)!
                        .Url
                    : DefaultProductImageUrl,
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
                CategoriesNames = this.GetCategoriesNames(),
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
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .SingleOrDefault(i => i.IsFrontImage)!
                        .Url
                    : DefaultProductImageUrl,
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
            FrontImageUrl = product.Images.Any()
                ? product.Images
                    .SingleOrDefault(i => i.IsFrontImage)!
                    .Url
                : DefaultProductImageUrl,
            ImagesUrls = product.Images
                .Select(i => i.Url),
        };
        
        return View(productDetailsViewModel);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        ProductCreateFormModel productCreateFormModel = new ProductCreateFormModel()
        {
            AllCategoriesForSelectInputFieldOptions = this.GetAllCategoriesViewModels()
        };
        
        return View(productCreateFormModel);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Create([FromForm] ProductCreateFormModel productCreateFormModel)
    {
        productCreateFormModel.AllCategoriesForSelectInputFieldOptions
            = this.GetAllCategoriesViewModels();
        
        bool isValidCategory = Guid.TryParse(productCreateFormModel.CategoryId,
            out Guid categoryGuidIdValue);
        bool categoryExists = productCreateFormModel.AllCategoriesForSelectInputFieldOptions
            .Any(c => c.Id == categoryGuidIdValue);
        if (!isValidCategory || !categoryExists)
        {
            string errorMessage = "Invalid category";
            ModelState.AddModelError(nameof(productCreateFormModel.CategoryId), errorMessage);
        }
        
        if (!ModelState.IsValid)
            return View(productCreateFormModel);
        
        try
        {
            Product newProduct = new Product()
            {
                Name = productCreateFormModel.ProductName,
                Description = productCreateFormModel.Description,
                QuantityInStock = productCreateFormModel.QuantityInStock,
                CostPrice = productCreateFormModel.CostPrice,
                SellingPrice = productCreateFormModel.SellingPrice,
                IsEnabled = productCreateFormModel.IsEnabled,
                OwnerId = this.GetUserId(),
                CategoryId = categoryGuidIdValue,
                Images = productCreateFormModel.FrontImageUrl != null
                    ? new HashSet<Image>()
                    {
                        new Image()
                        {
                            Url = productCreateFormModel.FrontImageUrl,
                            IsFrontImage = true,
                        }
                    }
                    : new HashSet<Image>(),
            };
        
            this._dbContext.Products.Add(newProduct);
            this._dbContext.SaveChanges();
            return RedirectToAction(nameof(Details), new { newProduct.Id });
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message);
            ViewData["ProductCreationError"] = ProductCreationUnexpectedErrorMessage;
            return View(productCreateFormModel);
        }
    }

    private IEnumerable<AllCategoriesViewModel> GetAllCategoriesViewModels()
    {
        return this._dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new AllCategoriesViewModel()
            {
                Id = c.Id,
                CategoryName = c.Name,
            })
            .ToArray();
    }

    private IEnumerable<string> GetCategoriesNames()
    {
        return this._dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => c.Name)
            .ToArray();
    }

    private Guid GetUserId()
    {
        string? userId = this.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Guid.Parse(userId);
    }
}