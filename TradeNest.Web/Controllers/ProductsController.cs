using TradeNest.Data;
using TradeNest.Data.Models;
using static TradeNest.Web.Utilities.OperationsStatusMessages;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Product;
using TradeNest.Web.ViewModels.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeNest.Services.Core.Interfaces;

namespace TradeNest.Web.Controllers;

[Authorize]
public class ProductsController : BaseController
{
    private readonly IProductsService _productsService;
    private readonly ICategoriesService _categoriesService;
    
    private TradeNestDbContext _dbContext;
    private ILogger<ProductsController> _logger;
    
    public ProductsController(TradeNestDbContext dbContext, ILogger<ProductsController> logger,
        IProductsService productsService, ICategoriesService categoriesService)
    {
        this._dbContext = dbContext;
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
        CatalogProductsAndCategoriesViewModel viewModel = await this._productsService
            .GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync();
        
        if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(categoryId))
        {
            viewModel.Products = await this._productsService
                .GetAllProductVmsOrderedByCreatedOnDescAsync();
        }
        else if (!string.IsNullOrWhiteSpace(search))
        {
            viewModel.IsSearchResultSet = true;
            viewModel.Products = await this._productsService
                .GetAllProdsVmsWithSearchQueryForNameAsync(search);
        }
        else if (!string.IsNullOrEmpty(categoryId) && Guid.TryParse(categoryId, out Guid id))
        {
            bool categoryExists = await this._categoriesService.CategoryExists(id);
            if (!categoryExists)
                return NotFound();

            viewModel.Products = await this._productsService
                .GetAllProdsVmsByCategoryAsync(id);
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
            .GetProductDetailsViewModelById(guidIdValue);
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
        bool idIsValidGuid = Guid.TryParse(id, out Guid guidIdValue);
        if (!idIsValidGuid)
            return BadRequest();
        
        Guid userId = this.GetUserId();

        try
        {
            ProductEditFormModel? model = await this._productsService
                .GetProductEditFormModel(userId, guidIdValue);
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
    public IActionResult Edit(
        [FromForm] ProductEditFormModel productEditFormModel,
        [FromRoute] string? id = null)
    {
        if(string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid productIdGuidValue)) 
            return BadRequest();

        Product? product = this._dbContext.Products
            .Include(p => p.Images)
            .SingleOrDefault(p => p.Id == productIdGuidValue);
        if (product == null)
            return NotFound();
        
        Guid userId = this.GetUserId();
        if (userId != product.OwnerId)
            return Unauthorized();

        productEditFormModel.AllCategoriesForSelectInputFieldOptions
            = this.GetAllCategoriesViewModels();
        
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
        
        if (productEditFormModel.ProductImages.Any())
        {
            bool allImagesAreValid = productEditFormModel.ProductImages
                .All(editImg => product.Images.Any(dbImg => dbImg.Id == editImg.Id));
            if (!allImagesAreValid)
                return BadRequest();
        }
        
        try
        {
            ICollection<Image> imagesToDelete
                = this.GetImagesForDeletionIfAny(productEditFormModel.ProductImages).ToArray();
            if (imagesToDelete.Any())
                this._dbContext.Images.RemoveRange(imagesToDelete);

            product.Name = productEditFormModel.ProductName;
            product.Description = productEditFormModel.Description;
            product.QuantityInStock = productEditFormModel.QuantityInStock;
            product.SellingPrice = productEditFormModel.SellingPrice;
            product.CostPrice = productEditFormModel.CostPrice;
            product.IsEnabled = productEditFormModel.IsEnabled;
            product.CategoryId = categoryIdGuidValue;

            if (!string.IsNullOrEmpty(productEditFormModel.NewImagesUrls))
            {
                IEnumerable<Image> imagesToAdd = this.ParseImagesInputOnImageAdding(
                    extraImagesUrls: productEditFormModel.NewImagesUrls,
                    productId: product.Id);
                
                this._dbContext.Images.AddRange(imagesToAdd);
            }

            this.EnsureProductHasFrontImage(imagesToDelete, product.Images);
            
            this._dbContext.SaveChanges();
            return RedirectToAction(nameof(Details), new { product.Id });
        }
        catch (Exception err)
        {
            this._logger.LogError(err.Message);
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
            this._productsService.DeleteProduct(userId, productIdGuidValue);

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

    private void EnsureProductHasFrontImage(ICollection<Image> imagesToDelete,
        ICollection<Image> productImages)
    {
        bool frontImageIsMarkedForDeletion = imagesToDelete.Any(i => i.IsFrontImage);
        bool productHasImagesLeft = productImages
            .Any(prodImgs => imagesToDelete.All(delImgs => delImgs.Id != prodImgs.Id));
            
        if (frontImageIsMarkedForDeletion && productHasImagesLeft)
        {
            /* first image that doesn't have state "Deleted" in the change tracker is made front image.
            (images with state "Added" and "Deleted" in the change tracker are included in the product's
            images collection at this point) */
            
            /* using the imagesToDelete collection directly since it's essentially the same reference
            no need for long inline linq expression */
            imagesToDelete.Single(i => i.IsFrontImage).IsFrontImage = false;
                
            productImages
                .First(img => imagesToDelete.All(delImg => img.Id != delImg.Id))
                .IsFrontImage = true;
        }

        // handles case where we edit product without images to now give it images
        bool isFrontImageSet = productImages.Any(i => i.IsFrontImage);
        if (!isFrontImageSet && productHasImagesLeft)
        {
            productImages
                .First(img => imagesToDelete.All(delImg => img.Id != delImg.Id))
                .IsFrontImage = true;
        }
    }

    private IEnumerable<Image> GetImagesForDeletionIfAny(
        IEnumerable<ImageViewModel> imagesComingFromEditForm)
    {
        IEnumerable<ImageViewModel> imageViewModelsMarkedForDeletion = imagesComingFromEditForm
            .Where(i => i.IsMarkedToStay == false)
            .ToArray();
        if (!imageViewModelsMarkedForDeletion.Any())
            return Array.Empty<Image>();

        ICollection<Image> imagesToDelete = new List<Image>();
        
        foreach (ImageViewModel imageViewModel in imageViewModelsMarkedForDeletion)
        {
            // already loaded in memory
            Image imageToDel = this._dbContext.Images.Find(imageViewModel.Id)!;
            imagesToDelete.Add(imageToDel);
        }

        return imagesToDelete;
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

    private IEnumerable<Image> ParseImagesInputOnImageAdding(string? frontImageUrl = null,
        string? extraImagesUrls = null, Guid? productId = null)
    {
        if (frontImageUrl == null && extraImagesUrls == null)
            return Array.Empty<Image>();
        
        ICollection<Image> images = new List<Image>();
        
        if (!string.IsNullOrEmpty(frontImageUrl))
        {
            images.Add(new Image()
            {
                Url = frontImageUrl.Trim(),
                IsFrontImage = true,
            });
        }
            
        if (!string.IsNullOrEmpty(extraImagesUrls))
        {
            IEnumerable<string> extraImagesUrlsSplit = extraImagesUrls
                .Split("\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (string imageUrl in extraImagesUrlsSplit)
            {
                images.Add(new Image()
                {
                    Url = imageUrl.Trim(),
                    IsFrontImage = false,
                });
            }
        }

        if (productId != null && productId != Guid.Empty)
        {
            foreach (Image image in images)
                image.ProductId = productId.Value;
        }

        return images;
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