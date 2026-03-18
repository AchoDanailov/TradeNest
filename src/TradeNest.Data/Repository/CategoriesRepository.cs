using System.Linq.Expressions;

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

    public override async Task<bool> DeleteAsync(Category entity)
    {
        this.DbContext.Remove(entity);
        int res = await this.DbContext.SaveChangesAsync();

        return res == 1;
    }

    public override async Task<bool> DeleteRangeAsync(Expression<Func<Category, bool>> filter)
    {
        IQueryable<Category> targetEntries = this.DbContext.Set<Category>().Where(filter);
        await targetEntries.ExecuteDeleteAsync();
        int res = await this.DbContext.SaveChangesAsync();
        
        return res == targetEntries.Count();
    }
}