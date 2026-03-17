using Microsoft.EntityFrameworkCore;

using TradeNest.Data.Models;
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
    
    public Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        return Task.FromResult(this._categoriesRepository.GetAllAsReadOnlyAsync()
            .GetAwaiter().GetResult()
            .Select(c => new CategoryDto()
            {
                Id = c.Id,
                CategoryName = c.Name,
            }));
    }

    public async Task<IEnumerable<CategoryWithBestSellerImageDto>>
        GetAllCategoriesWithBestSellerImageAsync()
    {
        IEnumerable<CategoryWithBestSellerImageDto> allCategoriesWithBestSellerImageDtos 
            = this._categoriesRepository.GetAllAsReadOnlyAsync()
                .GetAwaiter().GetResult()
                .Select(c => new CategoryWithBestSellerImageDto()
                {
                    Id = c.Id,
                    CategoryName = c.Name,
                })
                .ToArray();
        
        foreach (CategoryWithBestSellerImageDto category in allCategoriesWithBestSellerImageDtos)
        {
            Product? categoryBestSeller = await this._productsRepository
                .GetCategoryBestSeller(category.Id, include: p => p.Images);
            if (categoryBestSeller != null && categoryBestSeller.Images.Any())
            {
                category.BestSellerImageUrl = categoryBestSeller.Images
                    .Single(i => i.IsFrontImage).Url;
            }
        }

        return allCategoriesWithBestSellerImageDtos;
    }
}