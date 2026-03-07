using System.Globalization;
using Microsoft.EntityFrameworkCore;

using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models;
using static TradeNest.Services.Core.Utilities.ExceptionMessages;

namespace TradeNest.Services.Core;

public class ProductsService : IProductsService
{
    private readonly IRepository _repository;

    public ProductsService(IRepository repository)
    {
        this._repository = repository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsOrderedByDateOfCreationDescAsync(
        string? search = null)
    {
        IQueryable<Product> productQuery = this._repository
            .AllAsReadonly<Product>()
            .OrderByDescending(p => p.CreatedOn)
            .ThenBy(p => p.Name);
        if (!string.IsNullOrWhiteSpace(search))
        {
            productQuery = productQuery
                .Where(p => p.Name.ToLower().Contains(search) ||
                            p.Category.Name.ToLower().Contains(search));
        }
        
        return await productQuery
            .Select(p => new ProductDto()
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

    public async Task<IEnumerable<ProductDto>> GetAllProductsByCategoryIdAsync(
        Guid categoryId, 
        string? search = null)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(categoryId)));

        IQueryable<Product> productQuery = this._repository
            .AllAsReadonly<Product>()
            .Where(p => p.CategoryId == categoryId)
            .OrderByDescending(p => p.CreatedOn)
            .ThenBy(p => p.Name);
        if (!string.IsNullOrWhiteSpace(search))
        {
            productQuery = productQuery
                .Where(p => p.Name.ToLower().Contains(search) ||
                            p.Category.Name.ToLower().Contains(search));
        }
        
        return await productQuery
            .Select(p => new ProductDto()
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

    public async Task<IEnumerable<ProductDto>> GetAllProductsOrderedByOrdersCountDescAsync()
    {
        return await this._repository.All<Product>()
            .Include(p => p.ProductsOrders)
            .AsNoTracking()
            .OrderByDescending(p => p.ProductsOrders
                .Sum(po => po.ProductsQuantity))
            .ThenByDescending(p => p.CreatedOn)
            .Select(p => new ProductDto()
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

    public async Task<ProductDetailsDto?> GetProductDetailsByIdAsync(Guid id,
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
        
        ProductDetailsDto productDetailsDto = new ProductDetailsDto()
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

        return productDetailsDto;
    }

    public async Task<Guid> CreateProductAsync(Guid userId, ProductCreateDto productDto)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        
        Guid passedInCategoryId = productDto.CategoryId;
        if (passedInCategoryId == Guid.Empty)
        {
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage,
                nameof(productDto.CategoryId)));
        }

        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        bool categoryExists = await this._repository
            .ExistsAsync<Category>(c => c.Id == passedInCategoryId);
        if (!categoryExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(Category), passedInCategoryId));
        }
        
        ICollection<Image> images = this.ParseImagesInputOnImageAdding(
                frontImageUrl: productDto.FrontImageUrl,
                extraImagesUrls: productDto.ExtraIMagesUrls)
            .ToHashSet();
        if (images.Any() && !images.Any(i => i.IsFrontImage))
        {
            images.First().IsFrontImage = true;
        }
                
        Product newProduct = new Product()
        {
            Name = productDto.ProductName,
            Description = productDto.Description,
            QuantityInStock = productDto.QuantityInStock,
            CostPrice = productDto.CostPrice,
            SellingPrice = productDto.SellingPrice,
            IsEnabled = productDto.IsEnabled,
            OwnerId = userId,
            CategoryId = passedInCategoryId,
            Images = images
        };

        await this._repository.AddAsync<Product>(newProduct);
        await this._repository.SaveChangesAsync();

        return newProduct.Id;
    }

    public async Task<ProductEditDto?> GetProductForEditAsync(Guid userId, Guid id)
    {
        if (id == Guid.Empty)
            return null;

        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }
        
        Product? product = await this._repository.AllAsReadonly<Product>()
            .Include(p => p.Images)
            .SingleOrDefaultAsync(p => p.Id == id);
        if (product == null)
            return null;

        if (userId != product.OwnerId)
            throw new UnauthorizedOperationException(userId, nameof(Product), id);
        
        ProductEditDto productEditDto = new ProductEditDto()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            QuantityInStock = product.QuantityInStock,
            SellingPrice = product.SellingPrice,
            CostPrice = product.CostPrice,
            IsEnabled = product.IsEnabled,
            ProductImages = product.Images
                .Select(i => new ImageDto()
                {
                    Id = i.Id,
                    Url = i.Url
                })
                .ToList(),
            CategoryId = product.CategoryId,
        };

        return productEditDto;
    }

    public async Task DeleteProductAsync(Guid userId, Guid id)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if(id == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(id)));
        
        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Product? product = await this._repository.FindByIdAsync<Product>(id);
        if (product == null)
            throw new ResourceNotFoundException(nameof(Product), id);

        if (userId != product.OwnerId)
            throw new UnauthorizedOperationException(userId, nameof(Product), product.Id);

        if (product.IsDeleted)
        {
            throw new InvalidOperationException(
                string.Format(CantDeleteAlreadyDeletedProduct, userId, id));
        }
        
        product.IsDeleted = true;
        await this._repository.SaveChangesAsync();
    }

    public async Task EditProductAsync(Guid userId, ProductEditDto productEditDto)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        if (productEditDto.Id == Guid.Empty)
        {
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, 
                nameof(productEditDto.Id)));
        }
        
        if (productEditDto.CategoryId == Guid.Empty)
        {
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, 
                nameof(productEditDto.CategoryId)));
        }
        
        bool userExists = await this._repository
            .ExistsAsync<ApplicationUser>(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }
        
        Product? product = await this._repository.All<Product>()
            .Include(p => p.Images)
            .SingleOrDefaultAsync(p => p.Id == productEditDto.Id);
        if (product == null)
            throw new ResourceNotFoundException(nameof(Product), productEditDto.Id);

        if (userId != product.OwnerId)
            throw new UnauthorizedOperationException(userId, nameof(Product), productEditDto.Id);
        
        if (productEditDto.ProductImages.Any())
        {
            bool allImagesAreValid = productEditDto.ProductImages
                .All(editImg => product.Images.Any(dbImg => dbImg.Id == editImg.Id));
            if (!allImagesAreValid)
            {
                throw new ArgumentException("Invalid images provided",
                    nameof(productEditDto.ProductImages));
            }
        }

        bool categoryExists = await this._repository
            .ExistsAsync<Category>(c => c.Id == productEditDto.CategoryId);
        if (!categoryExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage, 
                nameof(Category), productEditDto.CategoryId));
        }

        ICollection<Image> imagesToDelete
            = await this.GetImagesForDeletionIfAny(productEditDto.ProductImages);
        if (imagesToDelete.Any())
            this._repository.RemoveRange<Image>(imagesToDelete);

        product.Name = productEditDto.Name;
        product.Description = productEditDto.Description;
        product.QuantityInStock = productEditDto.QuantityInStock;
        product.SellingPrice = productEditDto.SellingPrice;
        product.CostPrice = productEditDto.CostPrice;
        product.IsEnabled = productEditDto.IsEnabled;
        product.CategoryId = productEditDto.CategoryId;

        if (!string.IsNullOrEmpty(productEditDto.NewImagesUrls))
        {
            IEnumerable<Image> imagesToAdd = this.ParseImagesInputOnImageAdding(
                extraImagesUrls: productEditDto.NewImagesUrls,
                productId: product.Id);

            await this._repository.AddRangeAsync<Image>(imagesToAdd);
        }

        this.EnsureProductHasFrontImage(imagesToDelete, product.Images);
            
        await this._repository.SaveChangesAsync();
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
        IEnumerable<ImageDto> imagesComingFromEditForm)
    {
        IEnumerable<ImageDto> imageViewModelsMarkedForDeletion = imagesComingFromEditForm
            .Where(i => i.IsMarkedToStay == false)
            .ToArray();
        if (!imageViewModelsMarkedForDeletion.Any())
            return Array.Empty<Image>();

        ICollection<Image> imagesToDelete = new List<Image>();
        foreach (ImageDto imageViewModel in imageViewModelsMarkedForDeletion)
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