using Microsoft.EntityFrameworkCore;
using TradeNest.Data;
using TradeNest.Data.Models;
using TradeNest.GCommon;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Product;

namespace TradeNest.Services.Core;

public class ProductsService : IProductsService
{
    private readonly TradeNestDbContext _dbContext;

    public ProductsService(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }
    
    public CatalogProductsAndCategoriesViewModel GetEmptyCatalogProdsAndCategoriesDto()
    {
        return new CatalogProductsAndCategoriesViewModel(); 
    }

    public async Task<CatalogProductsAndCategoriesViewModel> GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync(
        IEnumerable<ProductViewModel>? productsViewModels = null)
    {
        CatalogProductsAndCategoriesViewModel viewModel
            = this.GetEmptyCatalogProdsAndCategoriesDto();
        
        viewModel.Categories = await this.GetAllCategoriesViewModels();

        if (productsViewModels != null)
            viewModel.Products = productsViewModels;

        return viewModel;
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProductVmsOrderedByCreatedOnDescAsync()
    {
        IEnumerable<ProductViewModel> productsViewModels = await this._dbContext.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedOn)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice.ToString(ApplicationConstants.PricesFormat),
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : ApplicationConstants.DefaultProductImageUrl,
                CategoryName = p.Category.Name,
            })
            .ToArrayAsync();

        return productsViewModels;
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProdsVmsWithSearchQueryForNameAsync(
        string searchQuery)
    {
        if (string.IsNullOrEmpty(searchQuery))
            return Array.Empty<ProductViewModel>();
        
        return await this._dbContext.Products
            .AsNoTracking()
            .Where(p => p.Name.Contains(searchQuery))
            .OrderByDescending(p => p.CreatedOn)
            .ThenBy(p => p.Name)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice.ToString(ApplicationConstants.PricesFormat),
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : ApplicationConstants.DefaultProductImageUrl,
                CategoryName = p.Category.Name,
            }) 
            .ToArrayAsync();
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProdsVmsByCategoryAsync(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            return Array.Empty<ProductViewModel>();
        
        return await this._dbContext.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedOn)
            .ThenBy(p => p.Name)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice.ToString(ApplicationConstants.PricesFormat),
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : ApplicationConstants.DefaultProductImageUrl,
                CategoryName = p.Category.Name,
            }) 
            .ToArrayAsync();
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProdsVmsOrderedByOrdersCountDescAsync()
    {
        return await this._dbContext.Products
            .AsNoTracking()
            .OrderByDescending(p => p.ProductsOrders.Count)
            .ThenByDescending(p => p.CreatedOn)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice.ToString(ApplicationConstants.PricesFormat),
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : ApplicationConstants.DefaultProductImageUrl,
                CategoryName = p.Category.Name,
            })
            .ToArrayAsync();
    }

    public async Task<bool> ProductExists(Guid id)
    {
        if (id == Guid.Empty)
            return false;

        return await this._dbContext.Products
            .AnyAsync(p => p.Id == id);
    }

    public async Task<ProductDetailsViewModel?> GetProductDetailsViewModelById(Guid id)
    {
        Product? product = await this._dbContext.Products
            .Include(p => p.Owner)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return null;
        
        ProductDetailsViewModel productDetailsViewModel = new ProductDetailsViewModel()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            QuantityInStock = product.QuantityInStock,
            SellingPrice = product.SellingPrice.ToString(ApplicationConstants.PricesFormat),
            IsEnabled = product.IsEnabled,
            Owner = product.Owner?.UserName ?? string.Empty,
            CategoryName = product.Category.Name,
            FrontImageUrl = product.Images.Any()
                ? product.Images
                    .Single(i => i.IsFrontImage)!.Url
                : ApplicationConstants.DefaultProductImageUrl,
            ImagesUrls = product.Images
                .Select(i => i.Url),
        };

        return productDetailsViewModel;
    }

    public ProductCreateFormModel GetEmptyProductCreateFormModel()
    {
        return new ProductCreateFormModel();
    }

    public async Task<ProductCreateFormModel> GetProdCreateFormModelWithLoadedCategoriesAsync()
    {
        ProductCreateFormModel productCreateFormModel =
            this.GetEmptyProductCreateFormModel();

        productCreateFormModel.AllCategoriesForSelectInputFieldOptions
            = await this.GetAllCategoriesViewModels();

        return productCreateFormModel;
    }

    public async Task<string> CreateProductAsync(Guid userId,
        ProductCreateFormModel productCreateFormModel)
    {
        string passedInCategoryId = productCreateFormModel.CategoryId;
        if (string.IsNullOrEmpty(passedInCategoryId) ||
            !Guid.TryParse(passedInCategoryId, out Guid categoryIdGuidValue))
        {
            throw new ArgumentException("Invalid category.",
                nameof(productCreateFormModel.CategoryId));
        }
        
        ICollection<Image> images = this.ParseImagesInputOnImageAdding(
                frontImageUrl: productCreateFormModel.FrontImageUrl,
                extraImagesUrls: productCreateFormModel.ExtraImagesUrls)
            .ToHashSet();
        if (images.Any() && !images.Any(i => i.IsFrontImage))
        {
            images.First().IsFrontImage = true;
        }
                
        Product newProduct = new Product()
        {
            Name = productCreateFormModel.ProductName,
            Description = productCreateFormModel.Description,
            QuantityInStock = productCreateFormModel.QuantityInStock,
            CostPrice = productCreateFormModel.CostPrice,
            SellingPrice = productCreateFormModel.SellingPrice,
            IsEnabled = productCreateFormModel.IsEnabled,
            OwnerId = userId,
            CategoryId = categoryIdGuidValue,
            Images = images
        };
        
        await this._dbContext.Products.AddAsync(newProduct);
        await this._dbContext.SaveChangesAsync();

        return newProduct.Id.ToString();
    }

    private async Task<IEnumerable<AllCategoriesViewModel>> GetAllCategoriesViewModels()
    {
        return await this._dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new AllCategoriesViewModel()
            {
                Id = c.Id,
                CategoryName = c.Name,
            })
            .ToArrayAsync();
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
}