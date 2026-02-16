using static TradeNest.Web.Utilities.OperationsStatusMessages;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeNest.Services.Core.Interfaces;

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
        [FromQuery] string? categoryId = null,
        [FromQuery] string? search = null)
    {
        CatalogProductsAndCategoriesViewModel viewModel;
        
        if (!string.IsNullOrEmpty(search))
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
            
            if (string.IsNullOrEmpty(categoryId))
            {
                viewModel.Products = await this._productsService
                    .GetAllProductsOrderedByDateOfCreationDescAsync();
            }
            else if (!string.IsNullOrEmpty(categoryId))
            {
                bool isValidCategory = this.IsValidCategory(categoryId,
                    viewModel.Categories, out Guid categoryIdGuidValue);
                if (!isValidCategory)
                    return NotFound();

                viewModel.Products = await this._productsService
                    .GetAllProdsVmsByCategoryIdAsync(categoryIdGuidValue);
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
    public async Task<IActionResult> Details([FromRoute] string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid prodIdGuidValue))
            return BadRequest();

        ProductDetailsViewModel? productDetailsViewModel = await this._productsService
            .GetProductDetailsViewModelByIdAsync(prodIdGuidValue);
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
    public async Task<IActionResult> Create([FromForm] ProductCreateFormModel productCreateFormModel)
    {
        productCreateFormModel.AllCategories
            = await this._categoriesService.GetAllCategoriesViewModelsAsync();

        bool isValidCategory = this.IsValidCategory(productCreateFormModel.CategoryId,
            productCreateFormModel.AllCategories, out _);
        if (!isValidCategory)
        {
            ModelState
                .AddModelError(nameof(productCreateFormModel.CategoryId), "Invalid category");
        }       
        
        if (!ModelState.IsValid)
            return View(productCreateFormModel);
        
        try
        {
            string productId = await this._productsService
                .CreateProductAsync(this.GetUserId(), productCreateFormModel);

            return RedirectToAction(nameof(Details), new { id = productId });
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message,
                "An unexpected error occured while trying to create a product.");
            ViewData["ProductCreationError"] = ProductCreationUnexpectedErrorMessage;
            
            return View(productCreateFormModel);
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit([FromRoute] string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guidIdValue))
            return BadRequest();
        
        try
        {
            Guid userId = this.GetUserId();
            ProductEditFormModel? model = await this._productsService
                .GetProductEditFormModelAsync(userId, guidIdValue);
            if (model == null)
                return NotFound();
        
            return View(model);
        }
        catch (Exception err)
        {
            this._logger.LogCritical(err.Message,
                $"An unexpected error occured while trying to access product data. Provided id: {id}.");

            return BadRequest();
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit([FromForm] ProductEditFormModel productEditFormModel,
        [FromRoute] string id, [FromForm] string? returnUrl)
    {
        if(string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid productIdGuidValue)) 
            return BadRequest();
        
        returnUrl ??= Url.Action(nameof(Index), controller: "Products");

        productEditFormModel.AllCategories = await this._categoriesService
            .GetAllCategoriesViewModelsAsync();
        
        bool isValidCategory = this.IsValidCategory(
            productEditFormModel.CategoryId,
            productEditFormModel.AllCategories,
            out _);
        if (!isValidCategory)
        {
            ModelState
                .AddModelError(nameof(productEditFormModel.CategoryId), "Invalid category");
        }

        if (!ModelState.IsValid)
            return View(productEditFormModel);

        bool productExists = await this._productsService
            .ProductExistsByIdAsync(productIdGuidValue);
        if (!productExists)
            return NotFound();

        try
        {
            Guid userId = this.GetUserId();
            await this._productsService
                .EditProductAsync(userId, productIdGuidValue, productEditFormModel);

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ArgumentException argsErr)
        {
            this._logger.LogError(argsErr.Message, 
                "An error occured due to invalid provided arguments while trying to modify the product. See internal error.");
            TempData["ProductModificationErrorMessage"] 
                = ProductModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message,
                "An unexpected error occured while trying modify the product.");
            TempData["ProductModificationErrorMessage"] 
                = ProductModificationUnexpectedErrorMessage;
            
            return LocalRedirect(returnUrl!);
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] string id, [FromForm] string? returnUrl)
    {
        if(string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid productIdGuidValue)) 
            return BadRequest();
        
        returnUrl ??= Url.Action(nameof(Index), controller: "Products");

        try
        {
            Guid userId = this.GetUserId();
            await this._productsService.DeleteProductAsync(userId, productIdGuidValue);

            TempData["ProductDeletionSuccessMessage"] = ProductDeletionSuccessMessage;
            return RedirectToAction(nameof(Index), controllerName: "Products");
        }
        catch (ArgumentException argsErr)
        {
            this._logger.LogError(argsErr.Message, 
                "An error occured due to invalid provided arguments while trying to delete the product. See internal error.");
            TempData["ProductModificationErrorMessage"] 
                = ProductModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message,
                "An unexpected error occured while trying to delete a product.");
            TempData["ProductModificationErrorMessage"]
                = ProductModificationUnexpectedErrorMessage;

            return LocalRedirect(returnUrl!);
        }
    }

    private bool IsValidCategory(string? id, IEnumerable<AllCategoriesViewModel> allCategoriesViewModels,
        out Guid categoryIdGuidValue)
    {
        if (string.IsNullOrEmpty(id))
        {
            categoryIdGuidValue = Guid.Empty;
            return false;
        }
        
        bool isValidGuidValue = Guid.TryParse(id, out Guid categoryIdValidGuidValue);
        bool categoryExists = allCategoriesViewModels.Any(c => c.Id == categoryIdValidGuidValue);
        
        categoryIdGuidValue = categoryIdValidGuidValue;
        return isValidGuidValue && categoryExists;
    }
}