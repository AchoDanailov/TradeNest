using Microsoft.EntityFrameworkCore;
using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;
using TradeNest.Services.Core.Interfaces;
using TradeNest.Web.ViewModels.Category;

namespace TradeNest.Services.Core;

public class CategoriesService : ICategoriesService
{
    private readonly IRepository _repository;

    public CategoriesService(IRepository repository)
    {
        this._repository = repository;
    }
    
    public async Task<bool> CategoryExistsByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            return false;
        
        return await this._repository.ExistsAsync<Category>(c => c.Id == id);
    }
    
    public async Task<IEnumerable<AllCategoriesViewModel>> GetAllCategoriesViewModelsAsync()
    {
        return await this._repository.All<Category>()
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
        return await this._repository.AllAsReadonly<Category>()
            .OrderBy(c => c.Name)
            .Select(c => new AllCategoriesWithBestSellerFrontImageViewModel()
            {
                Id = c.Id,
                CategoryName = c.Name,
                MostSoldProductFrontImageUrl = c.Products.Any() 
                    ? c.Products
                        .OrderByDescending(p => p.ProductsOrders
                            .Sum(po => po.ProductsQuantity))
                        .ThenByDescending(p => p.CreatedOn)
                        .First()
                        .Images.Any()
                        ? c.Products
                            .OrderByDescending(p => p.ProductsOrders
                                .Sum(po => po.ProductsQuantity))
                            .ThenByDescending(p => p.CreatedOn)
                            .First()
                            .Images.Single(i => i.IsFrontImage).Url
                        : null
                    : null
            })
            .ToArrayAsync();
    }
}