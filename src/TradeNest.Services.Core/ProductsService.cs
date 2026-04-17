using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon;
using TradeNest.GCommon.Exceptions;
using static TradeNest.GCommon.ErrorMessages;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Core.Mappers.Interfaces;
using TradeNest.Services.Models.Image;
using TradeNest.Services.Models.Product;
using static TradeNest.Services.Core.Utilities.ExceptionMessages;

namespace TradeNest.Services.Core;

public class ProductsService : IProductsService
{
    private readonly IProductsRepository _productsRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly IAdminsRepository _adminsRepository;
    private readonly IProductsMapper _productsMapper;

    public ProductsService(
        IProductsRepository productsRepository,
        IUsersRepository usersRepository,
        ICategoriesRepository categoriesRepository,
        IAdminsRepository adminsRepository,
        IProductsMapper productsMapper)
    {
        this._productsRepository = productsRepository;
        this._categoriesRepository = categoriesRepository;
        this._usersRepository = usersRepository;
        this._adminsRepository = adminsRepository;
        this._productsMapper = productsMapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsOrderedByDateOfCreationDescAsync(
        string? search = null)
    {
        IEnumerable<Product> products = await this._productsRepository
            .GetAllProductsWithCategoryAndImagesAsync(queryOptions =>
            {
                queryOptions
                    .AsReadOnly()
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
            .GetAllProductsWithCategoryAndImagesAsync(queryOptions =>
            {
                queryOptions
                    .AsReadOnly()
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
            .GetAllProductsWithCategoryAndImagesAsync(queryOptions => 
                queryOptions
                    .AsReadOnly()
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

    public async Task<int> GetSpecifiedProductsCountAsync(Guid userId, bool? approved = null,
        string? searchQuery = null)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        bool userIsAdmin = await this._adminsRepository.IsUserAdminByUserIdAsync(userId);
        if (!userIsAdmin)
            throw new UnauthorizedOperationException(userId, nameof(Product), "All identifiers");

        return await this._productsRepository
            .GetSpecifiedProductsCount(approved, searchQuery);
    }

    public async Task<IEnumerable<ProductDto2>> GetProductsDataWithPagination(Guid userId,
        int page, int limit, bool? approved = null, string? search = null)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if (page <= 0)
        {
            throw new ArgumentException(string.Format(CantBeZeroOrNegativeNumberMessage,
                nameof(page)));
        }
        if (limit <= 0)
        {
            throw new ArgumentException(string.Format(CantBeZeroOrNegativeNumberMessage,
                nameof(limit)));
        }
    
        bool isAdmin = await this._adminsRepository.IsUserAdminByUserIdAsync(userId);
        if (!isAdmin)
            throw new UnauthorizedOperationException(userId, nameof(Product), "All identifiers");

        Action<IQueryOptions<Product>> queryOptionsAction =
            (queryOptions) =>
            {
                queryOptions
                    .WithRelated(p => p.Owner)
                    .WithRelated(p => p.Category)
                    .AsReadOnly()
                    .AddOrderAsc(p => p.ApprovalDecision.ApprovalStatus)
                    .AddOrderDesc(p => p.CreatedOn)
                    .AddOrderAsc(p => p.Name)
                    .AddOrderAsc(p => p.Category.Name)
                    .AddOrderAsc(p => p.Id);
                if (!string.IsNullOrWhiteSpace(search))
                {
                    queryOptions
                        .AddFilter(p => p.Name.ToLower().Contains(search.ToLower()) ||
                                        p.Category.Name.ToLower().Contains(search.ToLower()));
                }

                queryOptions.WithPagination(page, limit);
            };

        IEnumerable<Product> products;
        if (approved is true)
        {
            products = await this._productsRepository
                .GetAllAsync(queryOptionsAction);
        }
        else
        {
            products = await this._productsRepository
                .GetAllInclNotApprovedAsync(queryOptionsAction);
            
            if (approved is false)
            {
                products = await this._productsRepository
                    .GetAllInclNotApprovedAsync(queryOptions =>
                    {
                        queryOptions
                            .WithRelated(p => p.Owner)
                            .WithRelated(p => p.Category)
                            .AsReadOnly()
                            .AddOrderAsc(p => p.ApprovalDecision.ApprovalStatus)
                            .AddOrderDesc(p => p.CreatedOn)
                            .AddOrderAsc(p => p.Name)
                            .AddOrderAsc(p => p.Category.Name)
                            .AddOrderAsc(p => p.Id);
                        if (!string.IsNullOrWhiteSpace(search))
                        {
                            queryOptions
                                .AddFilter(p => (p.Name.ToLower().Contains(search.ToLower()) ||
                                                 p.Category.Name.ToLower().Contains(search.ToLower())) &&
                                                p.ApprovalDecision.ApprovalStatus != ApprovalStatus.Approved);
                        }
                        else
                        {
                            queryOptions
                                .AddFilter(p => p.ApprovalDecision.ApprovalStatus != ApprovalStatus.Approved);
                        }

                        queryOptions.WithPagination(page, limit);
                    });
            }
        }

        return this._productsMapper.ToProductDtos2(products);
    }

    public async Task<IEnumerable<ProductDto2>> GetProductsData(
        Guid userId,
        bool? approved = null,
        string? search = null)
    {
        if(userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        
        bool isAdmin = await this._adminsRepository.IsUserAdminByUserIdAsync(userId);
        if (!isAdmin)
            throw new UnauthorizedOperationException(userId, nameof(Product), "All identifiers");
        
        Action<IQueryOptions<Product>> queryOptionsAction =
            (queryOptions) =>
            {
                queryOptions
                    .WithRelated(p => p.Owner)
                    .AsReadOnly()
                    .AddOrderAsc(p => p.ApprovalDecision.ApprovalStatus)
                    .AddOrderDesc(p => p.CreatedOn)
                    .AddOrderAsc(p => p.Name)
                    .AddOrderAsc(p => p.Id);
                if (!string.IsNullOrWhiteSpace(search))
                {
                    queryOptions
                        .AddFilter(p => p.Name.ToLower().Contains(search.ToLower()));
                }
            };

        IEnumerable<Product> products;
        if (approved is true)
        {
            products = await this._productsRepository
                .GetAllAsync(queryOptionsAction);
        }
        else
        {
            products = await this._productsRepository
                .GetAllInclNotApprovedAsync(queryOptionsAction);
            
            if (approved is false)
            {
                products = await this._productsRepository
                    .GetAllInclNotApprovedAsync(queryOptions =>
                    {
                        queryOptions
                            .WithRelated(p => p.Owner)
                            .AsReadOnly()
                            .AddOrderAsc(p => p.ApprovalDecision.ApprovalStatus)
                            .AddOrderDesc(p => p.CreatedOn)
                            .AddOrderAsc(p => p.Name)
                            .AddOrderAsc(p => p.Id);
                        if (!string.IsNullOrWhiteSpace(search))
                        {
                            queryOptions
                                .AddFilter(p => p.Name.ToLower().Contains(search.ToLower()) &&
                                                p.ApprovalDecision.ApprovalStatus != ApprovalStatus.Approved);
                        }
                        else
                        {
                            queryOptions
                                .AddFilter(p => p.ApprovalDecision.ApprovalStatus != ApprovalStatus.Approved);
                        }
                    });
            }
        }

        return this._productsMapper.ToProductDtos2(products);
    }

    public async Task<ProductDetailsDto?> GetProductDetailsByIdAsync(Guid id, Guid? userId = null)
    {
        if (id == Guid.Empty)
            return null;

        Product? product = await this._productsRepository
            .GetProductDetailsWithRelatedDataAsync(id, asReadOnly: true);
        if (product == null)
            return null;

        bool isOwner = userId != null && product.OwnerId == userId.Value;
        
        bool isAdmin = false;
        if(userId != null && userId.Value != Guid.Empty)
            isAdmin = await this._adminsRepository.IsUserAdminByUserIdAsync(userId.Value);
        
        if (!isOwner && !isAdmin)
            return null;
        
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

        ApplicationUser? user = await this._usersRepository.FindByIdAsync(userId);
        if (user == null)
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
        
        ICollection<Image> images = ParseImagesInputOnImageAdding(
                frontImageUrl: productDto.FrontImageUrl,
                extraImagesUrls: productDto.ExtraImagesUrls)
            .ToHashSet();
        if (images.Any() && !images.Any(i => i.IsFrontImage))
        {
            images.First().IsFrontImage = true;
        }

        Admin? admin = await this._adminsRepository.GetAdminByUserId(userId);
        
        Guid? approvalDecisionMakerId = admin?.Id ?? null;
        ApprovalDecision approvalDecision = new ApprovalDecision()
        {
            ApprovalStatus = admin != null ? ApprovalStatus.Approved : ApprovalStatus.WaitingApproval,
            DecisionJustification = null,
            TimeOfDecision = admin != null ? DateTime.UtcNow : null
        };
        
        Product newProduct = this._productsMapper.FromProductCreateDto(productDto, userId,
            images, approvalDecisionMakerId, approvalDecision);

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
                .GetAllInclArchivedAndNotApprovedAsync(queryOptions => 
                    queryOptions
                        .AsReadOnly()
                        .AddFilter(p => p.Id == id)))
            .SingleOrDefault();
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

        bool isAdmin = await this._adminsRepository.IsUserAdminByUserIdAsync(userId);
        if ((userId != product.OwnerId && !isAdmin) || !isAdmin)
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

        Product? product = (await this._productsRepository
                .GetAllInclArchivedAndNotApprovedAsync(queryOptions => 
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

        IEnumerable<Image> deletedImages 
            = DeleteImagesForDeletionIfAny(product, productEditDto.ProductImages);
        
        this._productsMapper.EditProductFromProductEditDto(productEditDto, product);

        AddNewImagesIfAny(product, productEditDto.NewImagesUrls);

        EnsureProductHasFrontImage(deletedImages, product.Images);

        bool updateProductResult = await this._productsRepository.UpdateAsync(product);
        if (updateProductResult == false)
        {
            throw new DataPersistException(nameof(updateProductResult),
                $"productId: {product.Id}");
        }
    }

    public async Task ChangeProductApprovalStatus(Guid userId, Guid productId,
        EditApprovalDecisionDto approvalDecisionDto)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, userId));
        if (productId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, productId));

        bool approvalStatusIsValidValue = Enum.TryParse(
            approvalDecisionDto.ApprovalStatus.ToString(),
            out ApprovalStatus validApprovalStatusValue); 
        bool isDefined = Enum.IsDefined(validApprovalStatusValue);
        if (!approvalStatusIsValidValue || !isDefined)
        {
            throw new ArgumentException($"userId: {userId}, productId: {productId}",
                nameof(approvalDecisionDto.ApprovalStatus));
        }

        Admin? admin = await this._adminsRepository.GetAdminByUserId(userId);
        if (admin == null)
            throw new UnauthorizedOperationException(userId, nameof(Product), productId);

        Product? product = (await this._productsRepository.GetAllInclNotApprovedAsync(queryOptions => 
                queryOptions.AddFilter(p => p.Id == productId)))
            .SingleOrDefault();
        if (product == null)
            throw new ResourceNotFoundException(nameof(Product), productId);

        product.ApprovalDecisionMakerId = admin.Id;
        product.ApprovalDecision = new ApprovalDecision()
        {
            ApprovalStatus = validApprovalStatusValue,
            DecisionJustification = approvalDecisionDto.DecisionJustification,
            TimeOfDecision = DateTime.UtcNow,
        };

        bool updateProductResult = await this._productsRepository.UpdateAsync(product);
        if (!updateProductResult)
        {
            throw new DataPersistException(nameof(updateProductResult),
                $"productId: {product.Id}");
        }
    }

    private static IEnumerable<Image> DeleteImagesForDeletionIfAny(Product product,
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

    private static void AddNewImagesIfAny(Product product, string? newImagesUrls)
    {
        if (string.IsNullOrWhiteSpace(newImagesUrls))
            return;
        
        IEnumerable<Image> imagesToAdd = ParseImagesInputOnImageAdding(
            extraImagesUrls: newImagesUrls,
            productId: product.Id);
        foreach (Image imageToAdd in imagesToAdd)
        {
            product.Images.Add(imageToAdd);
        }
    }
    
    private static void EnsureProductHasFrontImage(IEnumerable<Image> deletedImages,
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
    
    private static IEnumerable<Image> ParseImagesInputOnImageAdding(string? frontImageUrl = null,
        string? extraImagesUrls = null, Guid? productId = null)
    {
        if (frontImageUrl == null && extraImagesUrls == null)
            return Array.Empty<Image>();
        
        ICollection<Image> images = new List<Image>();
        
        if (!string.IsNullOrEmpty(frontImageUrl))
        {
            if (IsValidImageUrl(frontImageUrl))
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
                if (IsValidImageUrl(imageUrl))
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
   
    private static bool IsValidImageUrl(string? url)
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