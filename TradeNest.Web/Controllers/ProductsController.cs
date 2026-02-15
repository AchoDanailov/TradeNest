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
                .GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync(isFromSearchInput: true);
                
            viewModel.Products = await this._productsService
                .GetAllProdsVmsWithSearchQueryForNameAsync(search);
        }
        else
        {
            viewModel = await this._productsService
                .GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync();
            
            if (string.IsNullOrEmpty(categoryId))
            {
                viewModel.Products = await this._productsService
                    .GetAllProductVmsOrderedByCreatedOnDescAsync();
            }
            else if (!string.IsNullOrEmpty(categoryId) && Guid.TryParse(categoryId, out Guid id))
            {
                bool categoryExists = await this._categoriesService.CategoryExists(id);
                if (!categoryExists)
                    return NotFound();

                viewModel.Products = await this._productsService
                    .GetAllProdsVmsByCategoryAsync(id);
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
    public async Task<IActionResult> Details([FromRoute] string? id = null)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid guidIdValue))
            return BadRequest();

        ProductDetailsViewModel? productDetailsViewModel = await this._productsService
            .GetProductDetailsViewModelByIdAsync(guidIdValue);
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
        productCreateFormModel.AllCategoriesForSelectInputFieldOptions
            = await this._categoriesService.GetAllCategoriesViewModels();

        bool isValidCategory = this.IsValidCategory(productCreateFormModel.CategoryId,
            productCreateFormModel.AllCategoriesForSelectInputFieldOptions, out _);
        if (!isValidCategory)
        {
            ModelState
                .AddModelError(nameof(productCreateFormModel.CategoryId), "Invalid category");
        }       
        
        if (!ModelState.IsValid)
            return View(productCreateFormModel);
        
        try
        {
            string id = await this._productsService
                .CreateProductAsync(this.GetUserId(), productCreateFormModel);

            return RedirectToAction(nameof(Details), new { id });
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
    public async Task<IActionResult> Edit([FromRoute] string? id)
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
                $"An unexpected error occured while trying to access ProductEditFormModel for product with id: {id}.");

            return BadRequest();
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit([FromForm] ProductEditFormModel productEditFormModel,
        [FromRoute] string id)
    {
        if(string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid productIdGuidValue)) 
            return BadRequest();

        productEditFormModel.AllCategoriesForSelectInputFieldOptions
            = await this._categoriesService.GetAllCategoriesViewModels();
        
        bool isValidCategory = this.IsValidCategory(
            productEditFormModel.CategoryId,
            productEditFormModel.AllCategoriesForSelectInputFieldOptions,
            out Guid categoryIdGuidValue);
        if (!isValidCategory)
        {
            ModelState
                .AddModelError(nameof(productEditFormModel.CategoryId), "Invalid category");
        }

        if (!ModelState.IsValid)
            return View(productEditFormModel);

        bool productExists = await this._productsService.ProductExistsAsync(productIdGuidValue);
        if (!productExists)
            return NotFound();
        
        try
        {
            Guid userId = this.GetUserId();
            await this._productsService
                .EditProductAsync(userId, productIdGuidValue, productEditFormModel);
            
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message,
                "An unexpected error occured while trying modify the product.");
            ViewData["ProductModificationErrorMessage"] 
                = ProductModificationUnexpectedErrorMessage;
            
            return View(productEditFormModel);
        }
    }

    [HttpPost]
    [Authorize]
    public IActionResult Delete([FromRoute] string? id = null)
    {
        if(string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid productIdGuidValue)) 
            return BadRequest();
        
        Guid userId = this.GetUserId();

        try
        {
            this._productsService.DeleteProductAsync(userId, productIdGuidValue);

            TempData["ProductDeletionSuccessMessage"] = ProductDeletionSuccessMessage;
            return RedirectToAction(nameof(Index), controllerName: "Products");
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message,
                "An uncexpected error occured while trying to delete a product.");
            ViewData["ProductModificationErrorMessage"]
                = ProductModificationUnexpectedErrorMessage;

            return View(nameof(Details), new { id });
        }
    }

    /// <summary>
    /// This method checks if the the given id parameter's value can be parsed to valid Guid type
    /// and if it can be found as a value for the Id property of an instance in a collection
    /// with AllCategoriesViewModel type. 
    /// </summary>
    /// <param name="id">The category id string value</param>
    /// <param name="allCategoriesViewModels">Collection of AllCategoriesViewModel types</param>
    /// <param name="categoryIdGuidValue">Valid category id guid value</param>
    /// <returns>Value that represents if the passed in id parameter value can be parsed to
    /// valid Guid type and if there is an instance's property Id value that matches it.</returns>
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