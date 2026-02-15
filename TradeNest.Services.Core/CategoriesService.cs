using Microsoft.EntityFrameworkCore;
using TradeNest.Data;
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
    
    public async Task<bool> CategoryExists(Guid id)
    {
        if (id == Guid.Empty)
            return false;
        
        return await this._dbContext
            .Categories
            .AnyAsync(c => c.Id == id);
    }
    
    public async Task<IEnumerable<AllCategoriesViewModel>> GetAllCategoriesViewModels()
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
}