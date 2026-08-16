using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using TradeNest.Data.Repository.Interfaces;
using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;
using TradeNest.Data.QueryOptions;
using TradeNest.Data.QueryOptions.Interfaces;

namespace TradeNest.Data.Repository;

public class ProductsRepository : BaseReadRepository<Product>, IProductsRepository
{
    public ProductsRepository(TradeNestDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Product?> GetProductDetailsWithRelatedDataAsync(Guid productId,
        bool asReadOnly = false)
    {
        IQueryable<Product> queryable = this.DbContext.Products
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Owner)
            .Include(p => p.ApprovalDecisionMaker)
            .ThenInclude(a => a!.User); // => In cases of ApprovalDecisionMaker = null this does not fail. It just returns null! (always access property safely)
        if (asReadOnly)
            queryable = queryable.AsNoTracking();

        return await queryable.SingleOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<IDictionary<Guid, string?>>
        GetAllCategoriesBestSellersFrontImagesAsync(bool asReadOnly = false)
    {
        IQueryable<Category> queryable = this.DbContext.Categories;
        if (asReadOnly)
            queryable = queryable.AsNoTracking();

        return await queryable
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

    public async Task<IEnumerable<Product>> GetAllProductsWithCategoryAndImagesAsync(
        Action<IQueryOptions<Product>>? queryOptionsBuilder = null)
    {
        IQueryable<Product> queryable = this.DbContext.Products
            .Include(p => p.Images)
            .Include(p => p.Category);
        if (queryOptionsBuilder != null)
        {
            return await QueryOptionsTranslator<Product>
                .ToQueryable(queryable, queryOptionsBuilder)
                .ToArrayAsync();
        }

        return await queryable.ToArrayAsync();
    }

    public async Task<IEnumerable<Product>> GetAllInclArchivedAndNotApprovedAsync(
        Action<IQueryOptions<Product>>? queryOptionsBuilder = null)
    {
        if (queryOptionsBuilder == null)
        {
            return await this.DbContext.Products
                .IgnoreQueryFilters()
                .ToArrayAsync();
        }

        IQueryable<Product> noQueryFilterQueryable = this.DbContext.Products
            .IgnoreQueryFilters();

        return await QueryOptionsTranslator<Product>
            .ToQueryable(noQueryFilterQueryable, queryOptionsBuilder)
            .ToArrayAsync();
    }

    public async Task<IEnumerable<Product>> GetAllInclNotApprovedAsync(
        Action<IQueryOptions<Product>>? queryOptionsBuilder = null)
    {
        if (queryOptionsBuilder == null)
        {
            return await this.DbContext.Products
                .IgnoreQueryFilters()
                .Where(p => p.IsDeleted == false)
                .ToArrayAsync();
        }

        IQueryable<Product> queryable = this.DbContext.Products
            .IgnoreQueryFilters()
            .Where(p => p.IsDeleted == false);

        return await QueryOptionsTranslator<Product>
            .ToQueryable(queryable, queryOptionsBuilder)
            .ToArrayAsync();
    }

    public async Task<int> GetSpecifiedProductsCountAsync(bool? approved = null,
        string? search = null)
    {
        IQueryable<Product> queryable = this.DbContext.Products
            .IgnoreQueryFilters();

        if (approved is true)
        {
            queryable = queryable.Where(p =>
                p.IsDeleted == false && p.ApprovalDecision.ApprovalStatus == ApprovalStatus.Approved);
        }
        else
        {
            queryable = queryable.Where(p => p.IsDeleted == false);
            if (approved is false)
            {
                queryable = queryable
                    .Where(p => p.ApprovalDecision.ApprovalStatus != ApprovalStatus.Approved);
            }
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            queryable = queryable
                .Include(p => p.Category)
                .Where(p => p.Name.ToLower().Contains(search.ToLower()) ||
                            p.Category.Name.ToLower().Contains(search.ToLower()));
        }

        return await queryable
            .AsNoTracking()
            .CountAsync();
    }

    public async Task<bool> ExistsIncludingArchivedAndNotApprovedAsync(
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

    public async Task<bool> ExecuteUpdateProductsRangeCategoriesIdsAsync(
        Expression<Func<Product, bool>> filter,
        Guid newCategoryId)
    {
        int res = await this.DbContext.Products
            .IgnoreQueryFilters()
            .Where(filter)
            .ExecuteUpdateAsync(prop =>
                prop.SetProperty(p => p.CategoryId, newCategoryId));

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
