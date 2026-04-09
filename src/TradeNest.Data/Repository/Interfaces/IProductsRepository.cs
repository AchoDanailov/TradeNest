using System.Linq.Expressions;
using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IProductsRepository : IReadRepository<Product>
{
    Task<Product?> GetProductDetailsWithRelatedDataAsync(Guid productId, bool asReadOnly = false);

    Task<IEnumerable<Product>> GetAllInclArchivedAndNotApprovedAsync(
        Action<IQueryOptions<Product>> queryOptionsBuilder);
    
    Task<IDictionary<Guid, string?>> GetAllCategoriesBestSellersFrontImagesAsync(bool asReadOnly = false);

    Task<IEnumerable<Product>> GetAllProductsWithCategoryAndImagesAsync(
        Action<IQueryOptions<Product>>? queryOptionsBuilder = null);

    Task<bool> ExistsIncludingArchivedAndNotApprovedAsync(Expression<Func<Product, bool>> filter);

    Task<bool> AddAsync(Product product);

    Task<bool> AddRangeAsync(IEnumerable<Product> products);

    Task<bool> UpdateAsync(Product product);
    
    Task<bool> ArchiveAsync(Product product);
}