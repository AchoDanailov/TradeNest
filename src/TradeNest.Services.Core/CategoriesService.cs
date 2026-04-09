using TradeNest.Data.Repository.Interfaces;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Services.Models.Category;

namespace TradeNest.Services.Core;

public class CategoriesService : ICategoriesService
{
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IProductsRepository _productsRepository;

    public CategoriesService(ICategoriesRepository categoriesRepository,
        IProductsRepository productsRepository)
    {
        this._categoriesRepository = categoriesRepository;
        this._productsRepository = productsRepository;
    }
    
    public async Task<bool> CategoryExistsByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return false;

        return await this._categoriesRepository
            .ExistsAsync(c => c.Id == id);
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
}