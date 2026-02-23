using TradeNest.Data;
using TradeNest.Data.Models;
using TradeNest.GCommon;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Image;
using TradeNest.Web.ViewModels.Product;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Services.Core;

public class ProductsService : IProductsService
{
    private readonly TradeNestDbContext _dbContext;

    public ProductsService(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }
    
    public CatalogProductsAndCategoriesViewModel GetEmptyCatalogProdsAndCategoriesDto(
        bool isFromSearchInput = false)
    {
        return new CatalogProductsAndCategoriesViewModel()
        {
            IsSearchResultSet = isFromSearchInput,
        }; 
    }

    public async Task<CatalogProductsAndCategoriesViewModel> GetCatalogProdsAndCategoriesDtoWithLoadedCategoriesAsync(
        IEnumerable<ProductViewModel>? productsViewModels = null,
        bool isFromSearchInput = false)
    {
        CatalogProductsAndCategoriesViewModel viewModel
            = this.GetEmptyCatalogProdsAndCategoriesDto(isFromSearchInput);
        
        viewModel.Categories = await this.GetAllCategoriesViewModels();

        if (productsViewModels != null)
            viewModel.Products = productsViewModels;

        return viewModel;
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProductsOrderedByDateOfCreationDescAsync()
    {
        IEnumerable<ProductViewModel> productsViewModels = await this._dbContext.Products
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedOn)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice,
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : ApplicationConstants.DefaultProductImageUrl,
                CategoryName = p.Category.Name,
            })
            .ToArrayAsync();

        return productsViewModels;
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProdsBySearchQueryForNameAsync(
        string searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
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
                SellingPrice = p.SellingPrice,
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : ApplicationConstants.DefaultProductImageUrl,
                CategoryName = p.Category.Name,
            }) 
            .ToArrayAsync();
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProdsVmsByCategoryIdAsync(
        Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("categoryId can not be empty.", nameof(categoryId));
        
        return await this._dbContext.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedOn)
            .ThenBy(p => p.Name)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice, 
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
            .Include(p => p.ProductsOrders)
            .AsNoTracking()
            .OrderByDescending(p => p.ProductsOrders.Count)
            .ThenByDescending(p => p.CreatedOn)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice,
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : ApplicationConstants.DefaultProductImageUrl,
                CategoryName = p.Category.Name,
            })
            .ToArrayAsync();
    }

    public async Task<bool> ProductExistsByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return false;

        return await this._dbContext.Products
            .AnyAsync(p => p.Id == id);
    }

    public async Task<ProductDetailsViewModel?> GetProductDetailsViewModelByIdAsync(Guid id,
        Guid? userId = null)
    {
        if (id == Guid.Empty)
            return null;
        
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
            SellingPrice = product.SellingPrice,
            IsEnabled = product.IsEnabled,
            OwnerName = product.Owner.UserName ?? string.Empty,
            IsOwner = userId != null && product.OwnerId == userId.Value,
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

        productCreateFormModel.AllCategories
            = await this.GetAllCategoriesViewModels();

        return productCreateFormModel;
    }

    public async Task<Guid> CreateProductAsync(Guid userId,
        ProductCreateFormModel productCreateFormModel)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId can not be empty.", nameof(userId));
        
        Guid passedInCategoryId = productCreateFormModel.CategoryId;
        if (passedInCategoryId == Guid.Empty)
        {
            throw new ArgumentException("CategoryId can not be empty.",
                nameof(productCreateFormModel.CategoryId));
        }

        bool userExists = await this._dbContext.Users
            .AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException("User with the provided id was not found.",
                nameof(userId));
        }

        bool categoryExists = await this._dbContext.Categories
            .AnyAsync(c => c.Id == passedInCategoryId);
        if (!categoryExists)
        {
            throw new ArgumentException(
                $"Category with the provided categoryId does not exist. userId: {userId}",
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
            CategoryId = passedInCategoryId,
            Images = images
        };
        
        await this._dbContext.Products.AddAsync(newProduct);
        await this._dbContext.SaveChangesAsync();

        return newProduct.Id;
    }

    public async Task<ProductEditFormModel?> GetProductEditFormModelAsync(Guid userId, Guid id)
    {
        if (id == Guid.Empty)
            return null;

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId can not be empty.", nameof(userId));

        bool userExists = await this._dbContext.Users
            .AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException("User with the provided id was not found.",
                nameof(userId));
        }
        
        Product? product = await this._dbContext.Products
            .AsNoTracking()
            .Include(p => p.Images)
            .SingleOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return null;

        if (userId != product.OwnerId)
        {
            throw new InvalidOperationException(
                $"Unauthorized access attempt. userId: {userId}, productId: {product.Id}");
        }
        
        ProductEditFormModel productEditFormModel = new ProductEditFormModel()
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Description = product.Description,
            QuantityInStock = product.QuantityInStock,
            SellingPrice = product.SellingPrice,
            CostPrice = product.CostPrice,
            IsEnabled = product.IsEnabled,
            ProductImages = product.Images
                .Select(i => new ImageViewModel()
                {
                    Id = i.Id,
                    Url = i.Url
                })
                .ToList(),
            CategoryId = product.CategoryId,
            AllCategories = await this.GetAllCategoriesViewModels(),
        };

        return productEditFormModel;
    }

    public async Task DeleteProductAsync(Guid userId, Guid id)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException("UserId can not be empty.", nameof(userId));
        
        if(id == Guid.Empty)
            throw new ArgumentException("Id can not be empty.", nameof(id));
        
        bool userExists = await this._dbContext.Users
            .AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException("User with the provided id was not found.",
                nameof(userId));
        }
        
        Product? product = await this._dbContext.Products.FindAsync(id);
        if (product == null)
            throw new ArgumentException("Product not found.", nameof(id));

        if(userId != product.OwnerId)
            throw new InvalidOperationException($"Unauthorized operation attempt. userId: {userId}");

        if (product.IsDeleted)
        {
            throw new InvalidOperationException(
                $"Can not delete an already deleted product. userId: {userId}, productId: {id}");
        }
        
        product.IsDeleted = true;
        await this._dbContext.SaveChangesAsync();
    }

    public async Task EditProductAsync(Guid userId, Guid productId,
        ProductEditFormModel productEditFormModel)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId can not be empty.", nameof(userId));
            
        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId can not be empty.", nameof(productId));
        
        if (productEditFormModel.CategoryId == Guid.Empty)
        {
            throw new ArgumentException("CategoryId can not be empty.",
                nameof(productEditFormModel.CategoryId));
        }
        
        bool userExists = await this._dbContext.Users
            .AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException("User with the provided id was not found.",
                nameof(userId));
        }
        
        Product? product = await this._dbContext.Products
            .Include(p => p.Images)
            .SingleOrDefaultAsync(p => p.Id == productId);
        if (product == null)
            throw new ArgumentException("Product not found.", nameof(productId));
        
        if (userId != product.OwnerId)
            throw new InvalidOperationException($"Unauthorized access attempt. userId: {userId}");
        
        if (productEditFormModel.ProductImages.Any())
        {
            bool allImagesAreValid = productEditFormModel.ProductImages
                .All(editImg => product.Images.Any(dbImg => dbImg.Id == editImg.Id));
            if (!allImagesAreValid)
            {
                throw new ArgumentException(
                    $"Invalid images provided. productId: {productId}, userId: {userId}",
                    nameof(productEditFormModel.ProductImages));
            }
        }
        
        bool categoryExists = await this._dbContext.Categories
            .AnyAsync(c => c.Id == productEditFormModel.CategoryId);
        if (!categoryExists)
        {
            throw new ArgumentException($"Invalid category provided from user with id {userId}",
                nameof(productEditFormModel.CategoryId));
        }
        
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
        product.CategoryId = productEditFormModel.CategoryId;

        if (!string.IsNullOrEmpty(productEditFormModel.NewImagesUrls))
        {
            IEnumerable<Image> imagesToAdd = this.ParseImagesInputOnImageAdding(
                extraImagesUrls: productEditFormModel.NewImagesUrls,
                productId: product.Id);
                
            this._dbContext.Images.AddRange(imagesToAdd);
        }

        this.EnsureProductHasFrontImage(imagesToDelete, product.Images);
            
        await this._dbContext.SaveChangesAsync();
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
            if (this.IsValidImageUrl(frontImageUrl))
            {
                images.Add(new Image()
                {
                    Url = frontImageUrl.Trim(),
                    IsFrontImage = true,
                });
            }
        }
            
        if (!string.IsNullOrEmpty(extraImagesUrls))
        {
            IEnumerable<string> extraImagesUrlsSplit = extraImagesUrls
                .Split("\n", StringSplitOptions.RemoveEmptyEntries);

            foreach (string imageUrl in extraImagesUrlsSplit)
            {
                if (this.IsValidImageUrl(imageUrl))
                {
                    images.Add(new Image()
                    {
                        Url = imageUrl.Trim(),
                        IsFrontImage = false,
                    });
                }
            }
        }

        if (productId != null && productId != Guid.Empty)
        {
            foreach (Image image in images)
                image.ProductId = productId.Value;
        }

        return images;
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
    
    private void EnsureProductHasFrontImage(ICollection<Image> imagesToDelete,
        ICollection<Image> productImages)
    {
        bool isFrontImageMarkedForDeletion = imagesToDelete.Any(i => i.IsFrontImage);
        bool productHasImagesLeft = productImages
            .Any(prodImg => imagesToDelete.All(delImg => delImg.Id != prodImg.Id));
            
        if (isFrontImageMarkedForDeletion && productHasImagesLeft)
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
        // also acts as the final check
        bool isFrontImageSet = productImages.Any(i => i.IsFrontImage);
        if (!isFrontImageSet && productHasImagesLeft)
        {
            productImages
                .First(img => imagesToDelete.All(delImg => img.Id != delImg.Id))
                .IsFrontImage = true;
        }
    }

    private bool IsValidImageUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return false;

        return url.Length >= EntityValidationConstants.Image.UrlMinLengthValue &&
               url.Length <= EntityValidationConstants.Image.UrlMaxLengthValue &&
               (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase));
    }
}