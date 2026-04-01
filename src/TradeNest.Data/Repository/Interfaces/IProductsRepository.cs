using System.Linq.Expressions;
using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IProductsRepository : IReadRepository<Product> 
{
    Task<IDictionary<Guid, string?>> GetAllCategoriesBestSellersFrontImagesAsReadonlyAsync();

    Task<IEnumerable<Product>> GetAllProductsWithCategoryAndImagesAsReadonlyAsync(
        Action<IQueryOptions<Product>>? queryOptionsBuilder = null);

    Task<bool> ExistsIncludingArchivedAndNotApproved(Expression<Func<Product, bool>> filter);

    Task<bool> AddAsync(Product product);

    Task<bool> AddRangeAsync(IEnumerable<Product> products);

    Task<bool> UpdateAsync(Product product);
    
    Task<bool> ArchiveAsync(Product product);
}