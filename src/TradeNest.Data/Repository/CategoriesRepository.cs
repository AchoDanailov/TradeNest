using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class CategoriesRepository : BaseRepository<Category>, ICategoriesRepository
{
    public CategoriesRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }
}