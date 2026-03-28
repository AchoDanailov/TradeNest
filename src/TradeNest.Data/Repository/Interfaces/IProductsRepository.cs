using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IProductsRepository : IRepository<Product> 
{
    Task<IDictionary<Guid, string?>> GetAllCategoriesBestSellersFrontImagesAsReadonlyAsync();

    Task<IEnumerable<Product>> GetAllProductsWithCategoryAndImagesAsReadonlyAsync(
        Action<IQueryOptions<Product>>? queryOptionsBuilder = null);

    Task<bool> ArchiveAsync(Product product);
}