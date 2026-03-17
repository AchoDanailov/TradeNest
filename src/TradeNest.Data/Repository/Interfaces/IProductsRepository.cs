using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IProductsRepository : IRepository
{
    Task<Product?> GetCategoryBestSeller(Guid categoryId, params Func<Product, object>[] include);
}