using TradeNest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TradeNest.Data;

public class TradeNestDbContext 
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public TradeNestDbContext(DbContextOptions<TradeNestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; } = null!;
    public virtual DbSet<Image> Images { get; set; } = null!;
    public virtual DbSet<Order> Orders { get; set; } = null!;
    public virtual DbSet<OrderProduct> OrdersProducts { get; set; } = null!;
    public virtual DbSet<UsersWishlistProduct> UsersWishlistProducts { get; set; } = null!;
    public virtual DbSet<Category> Categories { get; set; } = null!;
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //ChangeTracker.Entries<TEntity>() calls DetectChanges() internally.
        IEnumerable<EntityEntry<Product>> productsToModifyIsEnabledProperty = this.ChangeTracker
            .Entries<Product>()
            .Where(entry => IsEnabledStatusShouldBeChanged(entry));

        foreach (EntityEntry<Product> productEntityEntry in productsToModifyIsEnabledProperty)
        {
            if (StockQuantityIsChangedTo0(productEntityEntry))
                productEntityEntry.Property(p => p.IsEnabled).CurrentValue = false;
            else
                productEntityEntry.Property(p => p.IsEnabled).CurrentValue = true;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }

    /// <summary>
    /// The method checks weather the product's entity quantityInStock property value has been modified from 0 (to some other number) or to 0 (from some other number).
    /// </summary>
    /// <param name="trackedProductEntityEntry">The EntityEntry type for the tracked product (provides access to change tracking information and operations).</param>
    /// <returns> bool - indicating weather the corresponding product record quantityInStock value has been modified to or from 0.</returns>
    private bool IsEnabledStatusShouldBeChanged(EntityEntry<Product> trackedProductEntityEntry)
    {
        PropertyEntry<Product, int> stockQuantityPropertyEntry = trackedProductEntityEntry.Property(p => p.QuantityInStock);
        bool isStockQuantityModified = stockQuantityPropertyEntry.IsModified;
        
        return (isStockQuantityModified && stockQuantityPropertyEntry.CurrentValue == 0) ||
               (isStockQuantityModified && stockQuantityPropertyEntry is { OriginalValue: 0, CurrentValue: > 0 });
    }

    /// <summary>
    /// The method checks weather the product's entity quantityInStock property value has been modified to 0 (from some other number).
    /// </summary>
    /// <param name="trackedProductEntityEntry">The EntityEntry type for the tracked product (provides access to change tracking information and operations).</param>
    /// <returns> bool - indicating weather the corresponding product record quantityInStock value has been modified to 0.</returns>
    private bool StockQuantityIsChangedTo0(EntityEntry<Product> trackedProductEntityEntry)
    {
        PropertyEntry<Product, int> stockQuantityPropertyEntry = trackedProductEntityEntry.Property(p => p.QuantityInStock);
        bool isStockQuantityModified = stockQuantityPropertyEntry.IsModified;

        return isStockQuantityModified && stockQuantityPropertyEntry.CurrentValue == 0;
    }
}