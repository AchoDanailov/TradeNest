using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models;
using TradeNest.Web.Utilities.Exceptions;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Image;
using TradeNest.Web.ViewModels.Product;
using static TradeNest.Web.Utilities.Messages.StatusNotificationMessages;
using static TradeNest.Web.Utilities.Messages.LoggingErrorMessages;

namespace TradeNest.Web.Controllers;

[Authorize]
public class ProductsController : BaseController
{
    private readonly IProductsService _productsService;
    private readonly ICategoriesService _categoriesService;
    private readonly ILogger<ProductsController> _logger;
    
    public ProductsController(ILogger<ProductsController> logger,
        IProductsService productsService, ICategoriesService categoriesService)
    {
        this._productsService = productsService;
        this._categoriesService = categoriesService;
        this._logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? search = null)
    {
        CatalogViewModel viewModel = new CatalogViewModel()
        {
            Categories = await this.GetAllCategoriesViewModelsAsync(),
        };

        if (!string.IsNullOrWhiteSpace(search))
        {
            if (search.ToLowerInvariant() == "all")
                search = null;
        }
        else
        {
            if(TempData["SearchFilter"] != null)
                search = TempData["SearchFilter"] as string ?? null;
        }
        
        TempData.Remove("SearchFilter");
        viewModel.SearchFilter = search;
        
        IEnumerable<ProductDto> productDtos = new List<ProductDto>();
        if (categoryId == null)
        {
            // no curr category filter control pressed
            
            // check if there is old category filtering 
            Guid? categoryFilter = TempData["CategoryFilter"] as Guid?;
            TempData.Remove("CategoryFilter");
            
            if (categoryFilter == null || categoryFilter.Value == Guid.Empty)
            {
                // no category filtering
                productDtos = await this._productsService
                    .GetAllProductsOrderedByDateOfCreationDescAsync(search);
            }
            else
            {
                // use old category filtering that was stored in the "CategoryFilter" prop of TempData
                productDtos = await this._productsService
                    .GetAllProductsByCategoryIdAsync(categoryFilter.Value, search);
            }

            // update view with the category filter status
            viewModel.CategoryFilter = categoryFilter;
        }
        else
        {
            // curr category filtering control pressed
            if (categoryId.Value == Guid.Empty)
            {
                //clear is pressed
                productDtos = await this._productsService
                    .GetAllProductsOrderedByDateOfCreationDescAsync(search);

                // update view with the category filter status
                viewModel.CategoryFilter = null;
            }
            else
            {
                // category is pressed
                bool isValidCategory 
                    = this.IsValidCategory(categoryId.Value, viewModel.Categories);
                if (!isValidCategory)
                    return NotFound();

                // update view with the category filter status
                viewModel.CategoryFilter = categoryId;
                
                productDtos = await this._productsService
                    .GetAllProductsByCategoryIdAsync(categoryId.Value, search);
            }
        }
        
        viewModel.Products = productDtos
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice,
                CategoryName = p.CategoryName,
                FrontImageUrl = p.FrontImageUrl,
            });

        return View(viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> BestSellers()
    {
        IEnumerable<ProductDto> productDtos = await this._productsService
            .GetAllProductsOrderedByOrdersCountDescAsync();

        CatalogViewModel viewModel = new CatalogViewModel()
        {
            Products = productDtos
                .Select(p => new ProductViewModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    SellingPrice = p.SellingPrice,
                    CategoryName = p.CategoryName,
                    FrontImageUrl = p.FrontImageUrl,
                }),
            Categories = await this.GetAllCategoriesViewModelsAsync(),
        };
        
        return View(nameof(Index), viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        Guid? userId = this.GetUserId();

        
        ProductDetailsDto? productDetailsDto = await this._productsService
            .GetProductDetailsByIdAsync(id, userId);
        if(productDetailsDto == null)
            return NotFound();

        ProductDetailsViewModel productDetailsViewModel = new ProductDetailsViewModel()
        {
            Id = productDetailsDto.Id,
            Name = productDetailsDto.Name,
            SellingPrice = productDetailsDto.SellingPrice,
            CategoryName = productDetailsDto.CategoryName,
            FrontImageUrl = productDetailsDto.FrontImageUrl,
            Description = productDetailsDto.Description,
            QuantityInStock = productDetailsDto.QuantityInStock,
            OwnerName = productDetailsDto.OwnerName,
            IsOwner = productDetailsDto.IsOwner,
            ImagesUrls = productDetailsDto.ImagesUrls,
            IsEnabled = productDetailsDto.IsEnabled,
        };
        
        return View(productDetailsViewModel);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        ProductCreateFormModel productCreateFormModel = new ProductCreateFormModel()
        {
            AllCategories = await this.GetAllCategoriesViewModelsAsync(),
        };
            
        
        return View(productCreateFormModel);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromForm] ProductCreateFormModel productCreateFormModel)
    {
        productCreateFormModel.AllCategories = await this.GetAllCategoriesViewModelsAsync();

        bool isValidCategory = this.IsValidCategory(
            productCreateFormModel.CategoryId,
            productCreateFormModel.AllCategories);
        if (!isValidCategory)
        {
            ModelState
                .AddModelError(nameof(productCreateFormModel.CategoryId), "Invalid category");
        }       
        
        if (!ModelState.IsValid)
            return View(productCreateFormModel);

        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
            {
                throw new UserIdMissingException(this.GetType().Name,
                    ControllerContext.ActionDescriptor.ActionName);
            }

            ProductCreateDto productCreateDto = new ProductCreateDto()
            {
                ProductName = productCreateFormModel.ProductName,
                SellingPrice = productCreateFormModel.SellingPrice,
                CategoryId = productCreateFormModel.CategoryId,
                FrontImageUrl = productCreateFormModel.FrontImageUrl,
                ExtraIMagesUrls = productCreateFormModel.ExtraImagesUrls,
                CostPrice = productCreateFormModel.CostPrice,
                Description = productCreateFormModel.Description,
                QuantityInStock = productCreateFormModel.QuantityInStock,
                IsEnabled = productCreateFormModel.IsEnabled,
            };

            Guid productId = await this._productsService
                .CreateProductAsync(userId.Value, productCreateDto);

            return RedirectToAction(nameof(Details), new { id = productId });
        }
        catch (ArgumentException argEx)
        {
            this._logger.LogWarning(argEx, string.Format(BadArgumentsErrorMessage, 
                this.GetType().Name, ControllerContext.ActionDescriptor.ActionName));
            
            ViewData["ProductCreationError"] = ProductCreationUnexpectedErrorMessage;
            return View(productCreateFormModel);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        Guid? userId = this.GetUserId();
        if (userId == null || userId.Value == Guid.Empty)
        {
            throw new UserIdMissingException(this.GetType().Name,
                ControllerContext.ActionDescriptor.ActionName);
        }
                
        ProductEditDto? model = await this._productsService
            .GetProductForEditAsync(userId.Value, id);
        if (model == null)
            return NotFound();
        
        ProductEditFormModel productEditFormModel = new ProductEditFormModel()
        {
            ProductId = model.Id,
            ProductName = model.Name,
            SellingPrice = model.SellingPrice,
            CategoryId = model.CategoryId,
            ProductImages = model.ProductImages
                .Select(i => new ImageViewModel()
                {
                    Id = i.Id,
                    Url = i.Url,
                    IsMarkedToStay = i.IsMarkedToStay,
                })
                .ToList(),
            CostPrice = model.CostPrice,
            Description = model.Description,
            QuantityInStock = model.QuantityInStock,
            IsEnabled = model.IsEnabled,
            AllCategories = await this.GetAllCategoriesViewModelsAsync(),
        };

        return View(productEditFormModel);
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit([FromForm] ProductEditFormModel productEditFormModel)
    {
        if(productEditFormModel.ProductId == Guid.Empty) 
            return BadRequest();

        productEditFormModel.AllCategories = await this.GetAllCategoriesViewModelsAsync();
        
        bool isValidCategory = this.IsValidCategory(
            productEditFormModel.CategoryId,
            productEditFormModel.AllCategories);
        if (!isValidCategory)
        {
            ModelState
                .AddModelError(nameof(productEditFormModel.CategoryId), "Invalid category");
        }

        if (!ModelState.IsValid)
            return View(productEditFormModel);

        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
            {
                throw new UserIdMissingException(this.GetType().Name,
                    ControllerContext.ActionDescriptor.ActionName);
            }

            ProductEditDto productEditDto = new ProductEditDto()
            {
                Id = productEditFormModel.ProductId,
                Name = productEditFormModel.ProductName,
                SellingPrice = productEditFormModel.SellingPrice,
                CategoryId = productEditFormModel.CategoryId,
                CostPrice = productEditFormModel.CostPrice,
                Description = productEditFormModel.Description,
                QuantityInStock = productEditFormModel.QuantityInStock,
                ProductImages = productEditFormModel.ProductImages
                    .Select(i => new ImageDto()
                    {
                        Id = i.Id,
                        Url = i.Url,
                        IsMarkedToStay = i.IsMarkedToStay,
                    }),
                NewImagesUrls = productEditFormModel.NewImagesUrls,
            };

            await this._productsService.EditProductAsync(userId.Value, productEditDto);

            return RedirectToAction(nameof(Details), new { id = productEditDto.Id });
        }
        catch (ArgumentException argEx)
        {
            this._logger.LogWarning(argEx, string.Format(BadArgumentsErrorMessage,
                this.GetType().Name, ControllerContext.ActionDescriptor.ActionName));
            
            TempData["ProductModificationErrorMessage"] 
                = ProductModificationUnexpectedErrorMessage;
            
            return View(productEditFormModel);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] Guid id, [FromForm] string? returnUrl)
    {
        if(id == Guid.Empty) 
            return BadRequest();
        
        returnUrl ??= Url.Action(nameof(Details), controller: "Products", new { id });

        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
            {
                throw new UserIdMissingException(this.GetType().Name,
                    ControllerContext.ActionDescriptor.ActionName);
            }

            await this._productsService.DeleteProductAsync(userId.Value, id);

            TempData["ProductDeletionSuccessMessage"] = ProductDeletionSuccessMessage;
            return RedirectToAction(nameof(Index), controllerName: "Products");
        }
        catch (ArgumentException argEx)
        {
            this._logger.LogWarning(argEx, string.Format(BadArgumentsErrorMessage, 
                this.GetType().Name, ControllerContext.ActionDescriptor.ActionName));
            
            TempData["ProductModificationErrorMessage"]
                = ProductModificationUnexpectedErrorMessage;
            
            return LocalRedirect(returnUrl!);
        }
    }

    private async Task<IEnumerable<AllCategoriesViewModel>> GetAllCategoriesViewModelsAsync()
    {
        IEnumerable<CategoryDto> allCategoriesDtos = await this._categoriesService
            .GetAllCategoriesAsync();
        
        return allCategoriesDtos
            .Select(c => new AllCategoriesViewModel()
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
            });
    }
    
    private bool IsValidCategory(Guid id,
        IEnumerable<AllCategoriesViewModel> allCategoriesViewModels)
    {
        bool isNotEmptyGuid = id != Guid.Empty;
        bool categoryExists = allCategoriesViewModels.Any(c => c.Id == id);
        
        return isNotEmptyGuid && categoryExists;
    }
}