using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Category;
using TradeNest.Services.Models.Product;
using static TradeNest.GCommon.ErrorMessages;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Product;
using TradeNest.Web.Mappers.Interfaces;
using static TradeNest.Web.Utilities.Messages.StatusNotificationMessages;

namespace TradeNest.Web.Controllers;

public class ProductsController : BaseController
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductsService _productsService;
    private readonly ICategoriesService _categoriesService;
    private readonly IProductPresentationModelsMapper _productPresentationModelsMapper;
    
    public ProductsController(
        ILogger<ProductsController> logger,
        IProductsService productsService,
        ICategoriesService categoriesService,
        IProductPresentationModelsMapper productPresentationModelsMapper)
    {
        this._logger = logger;
        this._productsService = productsService;
        this._categoriesService = categoriesService;
        this._productPresentationModelsMapper = productPresentationModelsMapper;
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/Catalog")] [Route("/Products")] [Route("/Products/Index")]
    public async Task<IActionResult> Index(
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? search = null)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            if (search.ToLowerInvariant() == "clear")
                search = null;
        }
        else
        {
            search = TempData["SearchFilter"] as string;
        }
        
        Guid? categoryFilter = categoryId ?? TempData["CategoryFilter"] as Guid?;
        if (categoryFilter == Guid.Empty)
            categoryFilter = null;
        
        TempData.Remove("SearchFilter");
        TempData.Remove("CategoryFilter");

        CatalogViewModel viewModel = new CatalogViewModel()
        {
            Categories = await this.GetAllCategoriesViewModelsAsync(),
            SearchFilter = search,
            CategoryFilter = categoryFilter,
        };
        
        if (categoryFilter.HasValue && 
            !IsValidCategory(categoryFilter.Value, viewModel.Categories))
        {
            return NotFound();
        }

        IEnumerable<ProductDto> productDtos;
        if (categoryFilter != null)
        {
            productDtos = await this._productsService
                .GetAllProductsByCategoryIdAsync(categoryFilter.Value, search);
        }
        else
        {
            productDtos = await this._productsService
                .GetAllProductsOrderedByDateOfCreationDescAsync(search);
        }
        productDtos = productDtos.ToList();

        viewModel.Products = this._productPresentationModelsMapper
            .ToProductViewModels(productDtos);
        
        return View(viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> BestSellers()
    {
        IEnumerable<ProductDto> productDtos = await this._productsService
            .GetAllProductsOrderedBySellingCountDescAsync();

        CatalogViewModel viewModel = new CatalogViewModel()
        {
            Products = this._productPresentationModelsMapper.ToProductViewModels(productDtos),
            Categories = await this.GetAllCategoriesViewModelsAsync(),
        };
        
        return View(nameof(Index), viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details([FromRoute] Guid id, 
        [FromQuery] string? returnUrl = null)
    {
        if (id == Guid.Empty)
            return BadRequest();

        Guid userId = this.GetUserId(throwIfNull: false);

        ProductDetailsDto? productDetailsDto = await this._productsService
            .GetProductDetailsByIdAsync(id, userId);
        if(productDetailsDto == null)
            return NotFound();

        if(string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl)) 
            returnUrl = Url.Action(nameof(Index), controller: "Products");

        ProductDetailsViewModel productDetailsViewModel = this._productPresentationModelsMapper
            .ToProductDetailsViewModel(productDetailsDto, returnUrl!);
        
        return View(productDetailsViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create([FromQuery] string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            returnUrl = Url.Action(nameof(Index), controller: "Products");
        
        ProductCreateFormModel productCreateFormModel = new ProductCreateFormModel()
        {
            AllCategories = await this.GetAllCategoriesViewModelsAsync(),
            ReturnUrl = returnUrl! 
        };
            
        return View(productCreateFormModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] ProductCreateFormModel productCreateFormModel)
    {
        productCreateFormModel.AllCategories = await this.GetAllCategoriesViewModelsAsync();

        bool isValidCategory = IsValidCategory(
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
            Guid userId = this.GetUserId(throwIfNull: true);

            ProductCreateDto productCreateDto = this._productPresentationModelsMapper
                .FromProductCreateFormModel(productCreateFormModel);

            Guid productId = await this._productsService
                .CreateProductAsync(userId, productCreateDto);

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
    public async Task<IActionResult> Edit([FromRoute] Guid id,
        [FromQuery] string? returnUrl = null)
    {
        if (id == Guid.Empty)
            return BadRequest();

        Guid userId = this.GetUserId(throwIfNull: true);
                
        ProductEditDto? model = await this._productsService.GetProductForEditAsync(userId, id);
        if (model == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            returnUrl = Url.Action(nameof(Index), controller: "Products");
        
        ProductEditFormModel productEditFormModel = this._productPresentationModelsMapper
            .ToProductEditFormModel(
                productEditDto: model,
                allCategories: (await this.GetAllCategoriesViewModelsAsync()).ToList(),
                returnUrl: returnUrl!);
        
        return View(productEditFormModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit([FromForm] ProductEditFormModel productEditFormModel)
    {
        if(productEditFormModel.ProductId == Guid.Empty) 
            return BadRequest();

        productEditFormModel.AllCategories = await this.GetAllCategoriesViewModelsAsync();
        
        bool isValidCategory = IsValidCategory(
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
            Guid userId = this.GetUserId(throwIfNull: true);

            ProductEditDto productEditDto = this._productPresentationModelsMapper
                .FromProductEditFormModel(productEditFormModel);

            await this._productsService.EditProductAsync(userId, productEditDto);

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
    public async Task<IActionResult> Delete([FromRoute] Guid id, 
        [FromForm] string? returnUrl = null)
    {
        if(id == Guid.Empty) 
            return BadRequest();
        
        if(returnUrl == null || !Url.IsLocalUrl(returnUrl))
            returnUrl = Url.Action(nameof(Index), controller: "Products");

        try
        {
            Guid userId = this.GetUserId(throwIfNull: true);

            await this._productsService.DeleteProductAsync(userId, id);

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
    
    private static bool IsValidCategory(Guid id,
        IEnumerable<AllCategoriesViewModel> allCategoriesViewModels)
    {
        bool isNotEmptyGuid = id != Guid.Empty;
        bool categoryExists = allCategoriesViewModels.Any(c => c.Id == id);
        
        return isNotEmptyGuid && categoryExists;
    }
}