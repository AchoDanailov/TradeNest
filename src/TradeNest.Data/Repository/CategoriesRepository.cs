using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class CategoriesRepository : BaseReadRepository<Category>, ICategoriesRepository
{
    public CategoriesRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }
    
    public async Task<bool> AddAsync(Category category)
    {
        await this.DbContext.Categories.AddAsync(category);
        int res = await this.DbContext.SaveChangesAsync();

        return res >= 1;
    }

    public async Task<bool> AddRangeAsync(IEnumerable<Category> categories)
    {
        await this.DbContext.Categories.AddRangeAsync(categories);
        int res = await this.DbContext.SaveChangesAsync();

        return res >= 1;
    }
}