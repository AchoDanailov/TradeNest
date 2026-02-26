using Microsoft.EntityFrameworkCore;
using TradeNest.GCommon;
using TradeNest.GCommon.Exceptions;
using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.ViewModels.Category;
using TradeNest.Web.ViewModels.Image;
using TradeNest.Web.ViewModels.Product;

namespace TradeNest.Services.Core;

public class ProductsService : IProductsService
{
    private readonly IRepository _repository;

    public ProductsService(IRepository repository)
    {
        this._repository = repository;
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
        IEnumerable<ProductViewModel> productsViewModels = await this._repository
            .AllAsReadonly<Product>()
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
                    : null,
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
        
        return await this._repository.AllAsReadonly<Product>()
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
                    : null,
                CategoryName = p.Category.Name,
            }) 
            .ToArrayAsync();
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProdsVmsByCategoryIdAsync(
        Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException("categoryId can not be empty.", nameof(categoryId));
        
        return await this._repository.AllAsReadonly<Product>()
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
                    : null,
                CategoryName = p.Category.Name,
            }) 
            .ToArrayAsync();
    }

    public async Task<IEnumerable<ProductViewModel>> GetAllProdsVmsOrderedByOrdersCountDescAsync()
    {
        return await this._repository.All<Product>()
            .Include(p => p.ProductsOrders)
            .AsNoTracking()
            .OrderByDescending(p => p.ProductsOrders
                .Sum(po => po.ProductsQuantity))
            .ThenByDescending(p => p.CreatedOn)
            .Select(p => new ProductViewModel()
            {
                Id = p.Id,
                Name = p.Name,
                SellingPrice = p.SellingPrice,
                FrontImageUrl = p.Images.Any()
                    ? p.Images
                        .Single(i => i.IsFrontImage)!.Url
                    : null,
                CategoryName = p.Category.Name,
            })
            .ToArrayAsync();
    }

    public async Task<bool> ProductExistsByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return false;

        return await this._repository
            .ExistsAsync<Product>(p => p.Id == id);
    }

    public async Task<ProductDetailsViewModel?> GetProductDetailsViewModelByIdAsync(Guid id,
        Guid? userId = null)
    {
        if (id == Guid.Empty)
            return null;
        
        Product? product = await this._repository.All<Product>()
            .Include(p => p.Owner)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .AsNoTracking()
            .AsSplitQuery()
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
                : null,
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

        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException($"User with id: {userId} was not found.",
                nameof(userId));
        }

        bool categoryExists = await this._repository
            .ExistsAsync<Category>(c => c.Id == passedInCategoryId);
        if (!categoryExists)
        {
            throw new ArgumentException(
                $"Category with id: {passedInCategoryId} was not found. userId: {userId}",
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

        await this._repository.AddAsync<Product>(newProduct);
        await this._repository.SaveChangesAsync();

        return newProduct.Id;
    }

    public async Task<ProductEditFormModel?> GetProductEditFormModelAsync(Guid userId, Guid id)
    {
        if (id == Guid.Empty)
            return null;

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId can not be empty.", nameof(userId));

        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException($"User with id: {userId} was not found.",
                nameof(userId));
        }
        
        Product? product = await this._repository.AllAsReadonly<Product>()
            .Include(p => p.Images)
            .SingleOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return null;

        if (userId != product.OwnerId)
            throw new UnauthorizedOperationException(userId, nameof(Product), id);
        
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
        
        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException($"User with id: {userId} was not found.",
                nameof(userId));
        }

        Product? product = await this._repository.FindByIdAsync<Product>(id);
        if (product == null)
            throw new ArgumentException($"Product with id: {id} was not found.", nameof(id));

        if (userId != product.OwnerId)
            throw new UnauthorizedOperationException(userId, nameof(Product), product.Id);

        if (product.IsDeleted)
        {
            throw new InvalidOperationException(
                $"Can not delete an already deleted product. userId: {userId}, productId: {id}");
        }
        
        product.IsDeleted = true;
        await this._repository.SaveChangesAsync();
    }

    public async Task EditProductAsync(Guid userId, ProductEditFormModel productEditFormModel)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId can not be empty.", nameof(userId));

        if (productEditFormModel.ProductId == Guid.Empty)
        {
            throw new ArgumentException("ProductId can not be empty.", 
                nameof(productEditFormModel.ProductId));
        }
        
        if (productEditFormModel.CategoryId == Guid.Empty)
        {
            throw new ArgumentException("CategoryId can not be empty.",
                nameof(productEditFormModel.CategoryId));
        }
        
        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException($"User with id: {userId} was not found.",
                nameof(userId));
        }
        
        Product? product = await this._repository.All<Product>()
            .Include(p => p.Images)
            .SingleOrDefaultAsync(p => p.Id == productEditFormModel.ProductId);
        if (product == null)
        {
            throw new ArgumentException($"Product with id: {productEditFormModel.ProductId} was not found.", 
                nameof(productEditFormModel.ProductId));
        }

        if (userId != product.OwnerId)
        {
            throw new UnauthorizedOperationException(userId, nameof(Product),
                productEditFormModel.ProductId);
        }
        
        if (productEditFormModel.ProductImages.Any())
        {
            bool allImagesAreValid = productEditFormModel.ProductImages
                .All(editImg => product.Images.Any(dbImg => dbImg.Id == editImg.Id));
            if (!allImagesAreValid)
            {
                throw new ArgumentException(
                    $"Invalid images provided. productId: {productEditFormModel.ProductId}, userId: {userId}",
                    nameof(productEditFormModel.ProductImages));
            }
        }

        bool categoryExists = await this._repository
            .ExistsAsync<Category>(c => c.Id == productEditFormModel.CategoryId);
        if (!categoryExists)
        {
            throw new ArgumentException($"Category with id: {productEditFormModel.CategoryId} was not found. userId: {userId}",
                nameof(productEditFormModel.CategoryId));
        }

        ICollection<Image> imagesToDelete
            = await this.GetImagesForDeletionIfAny(productEditFormModel.ProductImages);
        if (imagesToDelete.Any())
            this._repository.RemoveRange<Image>(imagesToDelete);

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

            await this._repository.AddRangeAsync<Image>(imagesToAdd);
        }

        this.EnsureProductHasFrontImage(imagesToDelete, product.Images);
            
        await this._repository.SaveChangesAsync();
    }

    private async Task<IEnumerable<AllCategoriesViewModel>> GetAllCategoriesViewModels()
    {
        return await this._repository.AllAsReadonly<Category>()
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
    
    private async Task<ICollection<Image>> GetImagesForDeletionIfAny(
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
            Image imageToDel = (await this._repository.FindByIdAsync<Image>(imageViewModel.Id))!;
            imagesToDelete.Add(imageToDel!);
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

        return url.Length >= EntityValidationConstants.CommonValidationConstants.UrlMinLengthValue &&
               url.Length <= EntityValidationConstants.CommonValidationConstants.UrlMaxLengthValue &&
               (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase));
    }
}