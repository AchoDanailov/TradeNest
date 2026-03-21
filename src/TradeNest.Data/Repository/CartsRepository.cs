using Microsoft.EntityFrameworkCore;

using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class CartsRepository : BaseRepository<Cart>, ICartsRepository
{
    public CartsRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }

    public async Task<Cart?> GetUserCartWithProductsDetailsAsync(Guid userId, bool asReadOnly = false)
    {
        IQueryable<Cart> query = this.DbContext.Carts
            .Include(c => c.CartProducts)
            .ThenInclude(cp => cp.Product);
        if (asReadOnly)
            query = query.AsNoTracking();
        
        return await query
            .SingleOrDefaultAsync(c => c.CartOwnerId == userId);
    }

    public async Task<Cart?> GetCartWithCartProductsByUserIdAsync(Guid userId)
    {
        return await this.DbContext.Carts
            .Include(c => c.CartProducts)
            .SingleOrDefaultAsync(c => c.CartOwnerId == userId);
    }

    public override async Task<bool> UpdateAsync(Cart entity)
    {
        if (entity.CartProducts.Any())
            return await base.UpdateAsync(entity);
        
        this.DbContext.Carts.Remove(entity);
        int res = await this.DbContext.SaveChangesAsync();
        return res > 0;
    }

    public async Task<bool> DeleteAsync(Cart cart)
    {
        this.DbContext.Carts.Remove(cart);
        int res = await this.DbContext.SaveChangesAsync();

        return res > 0;
    }
}