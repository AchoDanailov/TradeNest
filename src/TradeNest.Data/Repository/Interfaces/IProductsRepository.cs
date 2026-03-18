using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IProductsRepository : IRepository<Product> 
{
    Task<Product?> GetCategoryBestSeller(Guid categoryId);
}