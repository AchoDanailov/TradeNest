using Microsoft.EntityFrameworkCore;
using TradeNest.Data;
using TradeNest.GCommon;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.ViewModels.Category;

namespace TradeNest.Services.Core;

public class CategoriesService : ICategoriesService
{
    private readonly TradeNestDbContext _dbContext;

    public CategoriesService(TradeNestDbContext dbContext)
    {
        this._dbContext = dbContext;
    }
    
    public async Task<bool> CategoryExistsByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return false;
        
        return await this._dbContext
            .Categories
            .AnyAsync(c => c.Id == id);
    }
    
    public async Task<IEnumerable<AllCategoriesViewModel>> GetAllCategoriesViewModelsAsync()
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

    public async Task<IEnumerable<AllCategoriesWithBestSellerFrontImageViewModel>>
        GetAllCategoriesWithBestSellerImageVmAsync()
    {
        return await this._dbContext.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new AllCategoriesWithBestSellerFrontImageViewModel()
                {
                    Id = c.Id,
                    CategoryName = c.Name,
                    MostSoldProductFrontImageUrl = c.Products.Any() 
                        ? c.Products
                            .OrderByDescending(p => p.ProductsOrders.Count).First()
                            .Images.Any()
                            ? c.Products
                                .OrderByDescending(p => p.ProductsOrders.Count).First()
                                .Images.Single(i => i.IsFrontImage).Url
                            : ApplicationConstants.DefaultProductImageUrl
                        : ApplicationConstants.DefaultProductImageUrl
                })
                .ToArrayAsync();
    }
}