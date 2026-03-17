using Microsoft.EntityFrameworkCore;
using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class CategoriesRepository : BaseRepository<Category>, ICategoriesRepository
{
    public CategoriesRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }

    public override async Task<IEnumerable<Category>> GetAllAsReadOnlyAsync()
    {
        return await this.DbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToArrayAsync();
    }
}