using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.GCommon;
using TradeNest.GCommon.Exceptions;
using static TradeNest.GCommon.ErrorMessages;
using TradeNest.Services.Models.Category;
using TradeNest.Services.Core.Interfaces;
using static TradeNest.Services.Core.Utilities.ExceptionMessages;

namespace TradeNest.Services.Core;

public class CategoriesService : ICategoriesService
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IProductsRepository _productsRepository;
    private readonly IAdminsRepository _adminsRepository;

    public CategoriesService(
        ICategoriesRepository categoriesRepository,
        IProductsRepository productsRepository,
        IAdminsRepository adminsRepository)
    {
        this._categoriesRepository = categoriesRepository;
        this._productsRepository = productsRepository;
        this._adminsRepository = adminsRepository;
    }
    
    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        return (await this._categoriesRepository.GetAllAsync(queryOptions => 
                queryOptions
                    .AsReadOnly()
                    .AddOrderAsc(c => c.Name)))
            .Select(c => new CategoryDto()
            {
                Id = c.Id,
                CategoryName = c.Name,
            });
    }

    public async Task<IEnumerable<CategoryWithBestSellerImageDto>>
        GetAllCategoriesWithBestSellerImageAsync()
    {
        IEnumerable<CategoryWithBestSellerImageDto> allCategoriesWithBestSellerImageDtos
            = (await this._categoriesRepository.GetAllAsync(options => 
                options
                    .AsReadOnly()
                    .AddOrderAsc(c => c.Name)))
            .Select(c => new CategoryWithBestSellerImageDto()
            {
                Id = c.Id,
                CategoryName = c.Name,
            })
            .ToArray();

        IDictionary<Guid, string?> bestSellersFrontImagesByCategoryId 
            = await this._productsRepository.GetAllCategoriesBestSellersFrontImagesAsync(asReadOnly: true);
        foreach (CategoryWithBestSellerImageDto categoryDto in allCategoriesWithBestSellerImageDtos)
        {
            categoryDto.BestSellerImageUrl = bestSellersFrontImagesByCategoryId[categoryDto.Id];
        }

        return allCategoriesWithBestSellerImageDtos;
    }

    public async Task<Guid> CreateCategoryAsync(Guid userId, string categoryName)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException(string.Format(CantBeEmptyStringMessage, nameof(categoryName)));

        bool isAdmin = await this._adminsRepository.IsUserAdminByUserIdAsync(userId);
        if (!isAdmin)
        {
            throw new UnauthorizedOperationException(userId,
                nameof(Category), "Create new category");
        }

        bool createCategoryResult = await this._categoriesRepository
            .AddAsync(new Category() { Name = categoryName });
        if (!createCategoryResult)
        {
            throw new DataPersistException(nameof(createCategoryResult),
                $"category name: {categoryName}");
        }

        Category? category = (await this._categoriesRepository.GetAllAsync(queryOptions =>
                queryOptions
                    .AsReadOnly()
                    .AddFilter(c => c.Name == categoryName)))
            .SingleOrDefault();
        if (category == null)
        {
            throw new DataPersistException(nameof(createCategoryResult),
                $"category name: {categoryName}");
        }

        return category.Id;
    }

    public async Task<DeleteCategoryResultDto> DeleteCategoryByIdAsync(Guid userId, Guid categoryId)
    {
        if (categoryId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(categoryId)));
        if (userId == Guid.Empty)
            throw new ArgumentException(string.Format(IdCantBeEmptyMessage, nameof(userId)));

        Category? category = await this._categoriesRepository.FindByIdAsync(categoryId);
        if (category == null)
            throw new ResourceNotFoundException(nameof(Category), categoryId);
        
        bool isAdmin = await this._adminsRepository.IsUserAdminByUserIdAsync(userId);
        if (!isAdmin)
            throw new UnauthorizedOperationException(userId, nameof(Category), categoryId);

        bool categoryHasProducts = await this._productsRepository
            .ExistsIncludingArchivedAndNotApprovedAsync(p => p.CategoryId == categoryId);
        if (!categoryHasProducts)
        {
            bool deleteEmptyCategoryResult = await this._categoriesRepository
                .DeleteCategoryAsync(category);
            if (!deleteEmptyCategoryResult)
            {
                throw new DataPersistException(nameof(deleteEmptyCategoryResult), 
                    $"categoryId: {categoryId}");
            }
            
            return DeleteCategoryResultDto.Success();
        }

        Category? defaultCategory = (await this._categoriesRepository.GetAllAsync(queryOptions =>
                queryOptions.AddFilter(c => c.Name == ApplicationConstants.DefaultProductsCategory)))
            .SingleOrDefault();
        if (defaultCategory == null)
        {
            return DeleteCategoryResultDto
                .Failure(ExpectedFailureReason.NoCategoryToMoveProductsTo);
        }

        bool changeProductsCategoryIdsResult = await this._productsRepository
            .ExecuteUpdateRangeAsync<Guid>(
                filter: p => p.CategoryId == categoryId,
                updateProperty: p => p.CategoryId,
                updateValue: defaultCategory.Id);
        if (!changeProductsCategoryIdsResult)
        {
            throw new DataPersistException(nameof(changeProductsCategoryIdsResult), 
                $"categoryId: {categoryId}");
        }
        
        bool deleteCategoryResult = await this._categoriesRepository
            .DeleteCategoryAsync(category);
        if (!deleteCategoryResult)
        {
            throw new DataPersistException(nameof(deleteCategoryResult), 
                $"categoryId: {categoryId}");
        }
            
        return DeleteCategoryResultDto.Success();
    }
}