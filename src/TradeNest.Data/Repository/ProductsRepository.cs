using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using TradeNest.Data.Repository.Interfaces;
using TradeNest.Data.Models;

namespace TradeNest.Data.Repository;

public class ProductsRepository : BaseReadRepository<Product>, IProductsRepository
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
                BestSellerImage = c.Products
                    .OrderByDescending(p => p.SoldProducts.Sum(sp => sp.QuantityOrdered))
                    .Select(p => p.Images
                        .Where(i => i.IsFrontImage)
                        .Select(i => i.Url)
                        .SingleOrDefault())
                    .FirstOrDefault()
            })
            .ToDictionaryAsync(
                c => c.Id,
                c => c.BestSellerImage);
    }

    public async Task<IEnumerable<Product>> GetAllProductsWithCategoryAndImagesAsReadonlyAsync(
        Action<IQueryOptions<Product>>? queryOptionsBuilder = null)
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

    public async Task<bool> ExistsIncludingArchivedAndNotApproved(
        Expression<Func<Product, bool>> filter)
    {
        return await this.DbContext.Products
            .IgnoreQueryFilters()
            .AnyAsync(filter);
    }

    public async Task<bool> AddAsync(Product product)
    {
        await this.DbContext.Products.AddAsync(product);
        int res = await this.DbContext.SaveChangesAsync();

        return res >= 1;
    }

    public async Task<bool> AddRangeAsync(IEnumerable<Product> products)
    {
        await this.DbContext.Products.AddRangeAsync(products);
        int res = await this.DbContext.SaveChangesAsync();

        return res >= 1;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        this.DbContext.Products.Update(product);
        int res = await this.DbContext.SaveChangesAsync();

        return res >= 1;
    }

    public async Task<bool> ArchiveAsync(Product product)
    {
        product.IsDeleted = true;
        this.DbContext.Products.Update(product);
        
        int res = await this.DbContext.SaveChangesAsync();
        return res >= 1;
    }
}