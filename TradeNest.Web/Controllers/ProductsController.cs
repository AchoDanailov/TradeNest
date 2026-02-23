using static TradeNest.Web.Utilities.OperationsStatusMessages;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.Utilities;

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
        this._logger = logger;
        this._productsService = productsService;
        this._categoriesService = categoriesService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(
        [FromQuery] Guid? categoryId = null,
        [FromQuery] string? search = null)
    {
        CatalogProductsAndCategoriesViewModel viewModel;

        if (!string.IsNullOrWhiteSpace(search))
        {
            viewModel = await this._productsService
                .GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync(
                    isFromSearchInput: true);

            viewModel.Products = await this._productsService
                .GetAllProdsBySearchQueryForNameAsync(search);
        }
        else
        {
            viewModel = await this._productsService
                .GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync();

            if (categoryId == null || categoryId.Value == Guid.Empty)
            {
                viewModel.Products = await this._productsService
                    .GetAllProductsOrderedByDateOfCreationDescAsync();
            }
            else
            {
                bool isValidCategory = this.IsValidCategory(categoryId.Value,
                    viewModel.Categories);
                if (!isValidCategory)
                    throw new ArgumentException($"Invalid category provided: {categoryId}", nameof(categoryId)); 

                try
                {
                    viewModel.Products = await this._productsService
                        .GetAllProdsVmsByCategoryIdAsync(categoryId.Value);
                    
                    return View(viewModel);
                }
                catch (ArgumentException argEx)
                {
                    this._logger.LogError(argEx,
                        "Invalid arguments provided when attempting to access products and categories data. categoryId: ${CategoryId}",
                        categoryId);

                    viewModel.Products = await this._productsService
                        .GetAllProductsOrderedByDateOfCreationDescAsync();

                    ViewData["UnexpectedErrorMessage"] = OperationsStatusMessages.UnexpectedErrorMessage;
                    return View(viewModel);

                }
                catch (Exception ex)
                {
                    this._logger.LogCritical(ex,
                        "Unexpected exception occured while attempting to access products and categories data.");

                    return BadRequest();
                }
            }
        }

        return View(viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> BestSellers()
    {
        CatalogProductsAndCategoriesViewModel viewModel = await this._productsService
            .GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync();

        viewModel.Products = await this._productsService
            .GetAllProdsVmsOrderedByOrdersCountDescAsync();
        
        return View(nameof(Index), viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Details([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest();

        Guid? userId = this.GetUserId();

        ProductDetailsViewModel? productDetailsViewModel = await this._productsService
            .GetProductDetailsViewModelByIdAsync(id, userId);
        if (productDetailsViewModel == null)
            return NotFound();
        
        return View(productDetailsViewModel);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        ProductCreateFormModel productCreateFormModel = await this._productsService
            .GetProdCreateFormModelWithLoadedCategoriesAsync();
        
        return View(productCreateFormModel);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(
        [FromForm] ProductCreateFormModel productCreateFormModel)
    {
        productCreateFormModel.AllCategories
            = await this._categoriesService.GetAllCategoriesViewModelsAsync();

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
                throw new InvalidOperationException("UserId can not be null or empty.");
            
            Guid productId = await this._productsService
                .CreateProductAsync(userId.Value, productCreateFormModel);

            return RedirectToAction(nameof(Details), new { id = productId });
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex,
                "An unexpected error occured while trying to create a product.");
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

        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
                throw new InvalidOperationException("UserId can not be null or empty.");

            ProductEditFormModel? model = await this._productsService
                .GetProductEditFormModelAsync(userId.Value, id);
            if (model == null)
                return NotFound();

            return View(model);
        }
        catch (Exception ex)
        {
            this._logger.LogCritical(ex,
                "An unexpected error occured while trying to access product data. Provided productId: {Id}.",
                id);

            return BadRequest();
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit([FromForm] ProductEditFormModel productEditFormModel,
        [FromRoute] Guid id, [FromForm] string? returnUrl)
    {
        if(id == Guid.Empty) 
            return BadRequest();
        
        productEditFormModel.AllCategories = await this._categoriesService
            .GetAllCategoriesViewModelsAsync();
        
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

        returnUrl ??= Url.Action(nameof(Index), controller: "Products");

        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
                throw new InvalidOperationException("UserId can not be null or empty.");
            
            await this._productsService
                .EditProductAsync(userId.Value, id, productEditFormModel);

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex,
                "An unexpected error occured while trying modify the product with id: {Id}.",
                id);
            
            TempData["ProductModificationErrorMessage"] 
                = ProductModificationUnexpectedErrorMessage;
            
            return LocalRedirect(returnUrl!);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] Guid id, [FromForm] string? returnUrl)
    {
        if(id == Guid.Empty) 
            return BadRequest();
        
        returnUrl ??= Url.Action(nameof(Index), controller: "Products");

        try
        {
            Guid? userId = this.GetUserId();
            if (userId == null || userId.Value == Guid.Empty)
                throw new InvalidOperationException("UserId can not be null or empty.");
            
            await this._productsService.DeleteProductAsync(userId.Value, id);

            TempData["ProductDeletionSuccessMessage"] = ProductDeletionSuccessMessage;
            return RedirectToAction(nameof(Index), controllerName: "Products");
        }
        catch (Exception err)
        {
            this._logger.LogError(err,
                "An unexpected error occured while trying to delete a product.");
            TempData["ProductModificationErrorMessage"]
                = ProductModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
    }
    
    private bool IsValidCategory(Guid id,
        IEnumerable<AllCategoriesViewModel> allCategoriesViewModels)
    {
        bool isNotEmptyGuid = id != Guid.Empty;
        bool categoryExists = allCategoriesViewModels.Any(c => c.Id == id);
        
        return isNotEmptyGuid && categoryExists;
    }
}