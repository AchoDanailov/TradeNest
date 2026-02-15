using Microsoft.EntityFrameworkCore;
using TradeNest.Data;
using TradeNest.Services.Core.Interfaces;

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
}