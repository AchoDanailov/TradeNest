using System.Linq.Expressions;

using TradeNest.Data.Repository.Interfaces;
using TradeNest.Data.Models;

namespace TradeNest.Data.Repository;

public class ProductsRepository : BaseRepository<Product>, IProductsRepository
{
    public ProductsRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }

    public override async Task<bool> DeleteAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public override async Task<bool> DeleteRangeAsync(Expression<Func<Product, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public async Task<Product?> GetCategoryBestSeller(Guid categoryId)
    {
        throw new NotImplementedException();
    }
}