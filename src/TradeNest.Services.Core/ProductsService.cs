using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon;
using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Image;
using TradeNest.Services.Models.Product;
using static TradeNest.Services.Core.Utilities.ExceptionMessages;

namespace TradeNest.Services.Core;

public class ProductsService : IProductsService
{
    private readonly IProductsRepository _productsRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IProductsMapper _productsMapper;

    public ProductsService(
        IProductsRepository productsRepository,
        IUsersRepository usersRepository,
        ICategoriesRepository categoriesRepository,
        IProductsMapper productsMapper)
    {
        this._productsRepository = productsRepository;
        this._usersRepository = usersRepository;
        this._categoriesRepository = categoriesRepository;

        this._productsMapper = productsMapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsOrderedByDateOfCreationDescAsync(
        string? search = null)
    {
        IEnumerable<Product> products = await this._productsRepository
            .GetAllProductsWithCategoryAndImagesAsReadonlyAsync(queryOptions =>
            {
                queryOptions
                    .AddOrderDesc(p => p.CreatedOn)
                    .AddOrderAsc(p => p.Name);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    queryOptions
                        .AddFilter(p => p.Name.ToLower().Contains(search.ToLower()) ||
                                        p.Category.Name.ToLower().Contains(search.ToLower()));
                }
            });
        
        return this._productsMapper.ToProductDtos(products);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsByCategoryIdAsync(
        Guid categoryId, 
        string? search = null)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(categoryId)));

        IEnumerable<Product> products = await this._productsRepository
            .GetAllProductsWithCategoryAndImagesAsReadonlyAsync(queryOptions =>
            {
                queryOptions
                    .AddOrderDesc(p => p.CreatedOn)
                    .AddOrderAsc(p => p.Name);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    queryOptions
                        .AddFilter(p => p.CategoryId == categoryId &&
                                        (p.Name.ToLower().Contains(search.ToLower()) ||
                                         p.Category.Name.ToLower().Contains(search.ToLower())));
                }
                else
                {
                    queryOptions.AddFilter(p => p.CategoryId == categoryId);
                }
            });
        
        return this._productsMapper.ToProductDtos(products);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsOrderedBySellingCountDescAsync()
    {
        IEnumerable<Product> products = await this._productsRepository
            .GetAllProductsWithCategoryAndImagesAsReadonlyAsync(queryOptions => 
                queryOptions
                    .AddOrderDesc(p => p.SoldProducts.Sum(sp => sp.QuantityOrdered))
                    .AddOrderDesc(p => p.CreatedOn));
        
        return this._productsMapper.ToProductDtos(products);
    }

    public async Task<bool> ProductExistsByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return false;

        return await this._productsRepository
            .ExistsAsync(p => p.Id == id);
    }

    public async Task<ProductDetailsDto?> GetProductDetailsByIdAsync(Guid id, Guid? userId = null)
    {
        if (id == Guid.Empty)
            return null;

        Product? product = (await this._productsRepository.GetAllAsReadOnlyAsync(queryOptions =>
                queryOptions
                    .AddFilter(p => p.Id == id)
                    .WithRelated(p => p.Owner)
                    .WithRelated(p => p.Category)
                    .WithRelated(p => p.Images)))
            .FirstOrDefault();
        if (product == null)
            return null;

        bool isOwner = userId != null && product.OwnerId == userId.Value;
        return this._productsMapper.ToProductDetailsDto(product, isOwner);
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

        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        bool categoryExists = await this._categoriesRepository
            .ExistsAsync(c => c.Id == passedInCategoryId);
        if (!categoryExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(Category), passedInCategoryId));
        }
        
        ICollection<Image> images = this.ParseImagesInputOnImageAdding(
                frontImageUrl: productDto.FrontImageUrl,
                extraImagesUrls: productDto.ExtraImagesUrls)
            .ToHashSet();
        if (images.Any() && !images.Any(i => i.IsFrontImage))
        {
            images.First().IsFrontImage = true;
        }

        Product newProduct = this._productsMapper
            .FromProductCreateDto(productDto, userId, images);

        bool addProductResult = await this._productsRepository.AddAsync(newProduct);
        if (addProductResult == false)
        {
            throw new DataPersistException(nameof(addProductResult),
                $"{nameof(userId)}: {userId}");
        }
            
        return newProduct.Id;
    }

    public async Task<ProductEditDto?> GetProductForEditAsync(Guid userId, Guid id)
    {
        if (id == Guid.Empty)
            return null;

        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Product? product = (await this._productsRepository
                .GetAllProductsWithCategoryAndImagesAsReadonlyAsync(queryOptions =>
                    queryOptions.AddFilter(p => p.Id == id)))
            .FirstOrDefault();
        if (product == null)
            return null;

        if (userId != product.OwnerId)
            throw new UnauthorizedOperationException(userId, nameof(Product), id);
        
        return this._productsMapper.ToProductEditDto(product);
    }

    public async Task DeleteProductAsync(Guid userId, Guid id)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if(id == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(id)));
        
        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Product? product = await this._productsRepository.FindByIdAsync(id);
        if (product == null)
            throw new ResourceNotFoundException(nameof(Product), id);

        if (userId != product.OwnerId)
            throw new UnauthorizedOperationException(userId, nameof(Product), product.Id);

        if (product.IsDeleted)
        {
            throw new InvalidOperationException(
                string.Format(CantDeleteAlreadyDeletedProduct, userId, id));
        }

        bool archiveProductResult = await this._productsRepository.ArchiveAsync(product);
        if (archiveProductResult == false)
        {
            throw new DataPersistException(nameof(archiveProductResult), 
                $"{nameof(userId)}: {userId}", $"productId: {product.Id}");
        }
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
        
        bool userExists = await this._usersRepository.ExistsAsync(u => u.Id == userId);
        if (!userExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage,
                nameof(ApplicationUser), userId));
        }

        Product? product = (await this._productsRepository.GetAllAsync(queryOptions => 
                queryOptions
                    .WithRelated(p => p.Images)
                    .AddFilter(p => p.Id == productEditDto.Id)))
            .SingleOrDefault();
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

        bool categoryExists = await this._categoriesRepository
            .ExistsAsync(c => c.Id == productEditDto.CategoryId);
        if (!categoryExists)
        {
            throw new ArgumentException(string.Format(NotFoundMessage, 
                nameof(Category), productEditDto.CategoryId));
        }

        IEnumerable<Image> deletedImages = this.DeleteImagesForDeletionIfAny(
            product, productEditDto.ProductImages);
        
        this._productsMapper.EditProductFromProductEditDto(productEditDto, product);

        this.AddNewImagesIfAny(product, productEditDto.NewImagesUrls);

        this.EnsureProductHasFrontImage(deletedImages, product.Images);

        bool updateProductResult = await this._productsRepository.UpdateAsync(product);
        if (updateProductResult == false)
        {
            throw new DataPersistException(nameof(updateProductResult),
                $"productId: {product.Id}");
        }
    }

    private IEnumerable<Image> DeleteImagesForDeletionIfAny(Product product,
        IEnumerable<ImageDto> imagesComingFromEditForm)
    {
        IEnumerable<ImageDto> imageViewModelsMarkedForDeletion = imagesComingFromEditForm
            .Where(i => i.IsMarkedToStay == false)
            .ToArray();
        if (!imageViewModelsMarkedForDeletion.Any())
            return Array.Empty<Image>();

        ICollection<Image> deletedImages = new List<Image>();
        foreach (ImageDto deleteImageDto in imageViewModelsMarkedForDeletion)
        {
            Image? deleteImage = product.Images
                .SingleOrDefault(i => i.Id == deleteImageDto.Id);
            if (deleteImage != null)
            {
                deletedImages.Add(deleteImage);
                product.Images.Remove(deleteImage);
            }
        }

        return deletedImages;
    }

    private void AddNewImagesIfAny(Product product, string? newImagesUrls)
    {
        if (string.IsNullOrWhiteSpace(newImagesUrls))
            return;
        
        IEnumerable<Image> imagesToAdd = this.ParseImagesInputOnImageAdding(
            extraImagesUrls: newImagesUrls,
            productId: product.Id);
        foreach (Image imageToAdd in imagesToAdd)
        {
            product.Images.Add(imageToAdd);
        }
    }
    
    private void EnsureProductHasFrontImage(IEnumerable<Image> deletedImages,
        ICollection<Image> productImages)
    {
        deletedImages = deletedImages.ToArray();
        
        bool isFrontImageMarkedForDeletion = deletedImages.Any(i => i.IsFrontImage);
        bool productHasImagesLeft = productImages
            .Any(prodImg => deletedImages.All(delImg => delImg.Id != prodImg.Id));
            
        if (isFrontImageMarkedForDeletion && productHasImagesLeft)
        {
            deletedImages.Single(i => i.IsFrontImage).IsFrontImage = false;
                
            productImages
                .First(img => deletedImages.All(delImg => img.Id != delImg.Id))
                .IsFrontImage = true;
        }

        bool isFrontImageSet = productImages.Any(i => i.IsFrontImage);
        if (!isFrontImageSet && productHasImagesLeft)
        {
            productImages
                .First(img => deletedImages.All(delImg => img.Id != delImg.Id))
                .IsFrontImage = true;
        }
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