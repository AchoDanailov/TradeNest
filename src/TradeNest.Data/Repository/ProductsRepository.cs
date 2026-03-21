using Microsoft.EntityFrameworkCore;

using TradeNest.Data.Repository.Interfaces;
using TradeNest.Data.Models;

namespace TradeNest.Data.Repository;

public class ProductsRepository : BaseRepository<Product>, IProductsRepository
{
    public ProductsRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }

    public async Task<IDictionary<Guid, string?>> 
        GetAllCategoriesBestSellersFrontImagesAsReadonlyAsync()
    {
        return await this.DbContext.Categories
            .AsNoTracking()
            .Select(c => new
            {
                Id = c.Id,
                BestSellerImage = c.Products.OrderByDescending(p => p.SoldProducts
                        .Sum(sp => sp.QuantityOrdered))
                    .FirstOrDefault()!
                    .Images.SingleOrDefault(i => i.IsFrontImage)!.Url ?? null
            })
            .ToDictionaryAsync(
                c => c.Id,
                c => c.BestSellerImage);
    }

    // TODO: debug this.
    public async Task<IEnumerable<Product>> GetAllProductsWithCategoryAndImagesAsReadonlyAsync(
        Action<IQueryBuilder<Product>>? queryOptionsBuilder = null)
    {
        IQueryable<Product> queryable = this.DbContext.Products
            .Include(p => p.Images)
            .Include(p => p.Category);
        if (queryOptionsBuilder != null)
        {
            return await this.BuildQuery(queryOptionsBuilder, queryable, asReadOnly: true)
                .ToArrayAsync();
        }
        
        return await queryable
            .AsNoTracking()
            .ToArrayAsync();
    }

    public async Task<bool> ArchiveAsync(Product product)
    {
        product.IsDeleted = true;
        this.DbContext.Products.Update(product);
        
        int res = await this.DbContext.SaveChangesAsync();
        return res >= 1;
    }
}