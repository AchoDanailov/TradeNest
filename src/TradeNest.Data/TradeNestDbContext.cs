using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using TradeNest.Data.Models;
using TradeNest.Data.Models.Enums;

namespace TradeNest.Data;

public class TradeNestDbContext 
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public TradeNestDbContext(DbContextOptions<TradeNestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; } = null!;
    public virtual DbSet<Product> Products { get; set; } = null!;
    public virtual DbSet<Category> Categories { get; set; } = null!;
    public virtual DbSet<Image> Images { get; set; } = null!;
    public virtual DbSet<Order> Orders { get; set; } = null!;
    public virtual DbSet<OrderProduct> OrdersProducts { get; set; } = null!;
    public virtual DbSet<Cart> Carts { get; set; } = null!;
    public virtual DbSet<CartProduct> CartsProducts { get; set; } = null!;
    public virtual DbSet<UserWatchlistProduct> UsersWatchlistsProducts { get; set; } = null!;
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        //ChangeTracker.Entries<TEntity>() calls DetectChanges() internally.
        IEnumerable<EntityEntry<Product>> productsToModifyIsEnabledProperty = this.ChangeTracker
            .Entries<Product>()
            .Where(entry => InStockStatusChanged(entry) || HasChangedApprovalStatus(entry));
                            
        UpdateProductIsEnabledPropertyAsRequired(productsToModifyIsEnabledProperty);

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
    
    private static void UpdateProductIsEnabledPropertyAsRequired(
        IEnumerable<EntityEntry<Product>> productsToModifyIsEnabledProperty)
    {
        foreach (EntityEntry<Product> productEntityEntry in productsToModifyIsEnabledProperty)
        {
            if (WentOutOfStock(productEntityEntry) || IsNotApproved(productEntityEntry))
                productEntityEntry.Property(p => p.IsEnabled).CurrentValue = false;
            else
                productEntityEntry.Property(p => p.IsEnabled).CurrentValue = true;
        }
    }
    
    private static bool InStockStatusChanged(EntityEntry<Product> trackedProductEntityEntry)
    {
        PropertyEntry<Product, int> stockQuantityPropertyEntry 
            = trackedProductEntityEntry.Property(p => p.QuantityInStock);
        
        bool isStockQuantityModified = stockQuantityPropertyEntry.IsModified;
        
        return (isStockQuantityModified && stockQuantityPropertyEntry.CurrentValue == 0) ||
               (isStockQuantityModified && stockQuantityPropertyEntry is { OriginalValue: 0, CurrentValue: > 0 });
    }

    private static bool WentOutOfStock(EntityEntry<Product> trackedProductEntityEntry)
    {
        return trackedProductEntityEntry
            .Property(p => p.QuantityInStock)
            .CurrentValue == 0;
    }

    private static bool HasChangedApprovalStatus(EntityEntry<Product> productEntityEntry)
    {
        return productEntityEntry
            .Reference(p => p.ApprovalDecision).TargetEntry!
            .Property(d => d.ApprovalStatus)
            .IsModified;
    }

    private static bool IsNotApproved(EntityEntry<Product> productEntityEntry)
    {
        return productEntityEntry
            .Reference(p => p.ApprovalDecision).TargetEntry!
            .Property(d => d.ApprovalStatus)
            .CurrentValue != ApprovalStatus.Approved;
    }
}