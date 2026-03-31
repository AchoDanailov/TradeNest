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
            .Where(entry => ChangedInStockStatus(entry) || ProductIsNotApproved(entry));
        
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
            if (StockQuantityIsChangedTo0(productEntityEntry) ||
                ProductIsNotApproved(productEntityEntry))
            {
                productEntityEntry.Property(p => p.IsEnabled).CurrentValue = false;
            }
            else
            {
                productEntityEntry.Property(p => p.IsEnabled).CurrentValue = true;
            }
        }
    }

    private static bool ProductIsNotApproved(EntityEntry<Product> entry)
    {
        bool isModified = entry.Property(p => p.ApprovalStatus).IsModified;

        bool statusDiffFromApproved = entry.Entity.ApprovalStatus == ApprovalStatus.NotApproved ||
                                      entry.Entity.ApprovalStatus == ApprovalStatus.WaitingApproval;

        return isModified && statusDiffFromApproved;
    }
    
    private static bool ChangedInStockStatus(EntityEntry<Product> trackedProductEntityEntry)
    {
        PropertyEntry<Product, int> stockQuantityPropertyEntry 
            = trackedProductEntityEntry.Property(p => p.QuantityInStock);
        bool isStockQuantityModified = stockQuantityPropertyEntry.IsModified;
        
        return (isStockQuantityModified && stockQuantityPropertyEntry.CurrentValue == 0) ||
               (isStockQuantityModified && stockQuantityPropertyEntry is { OriginalValue: 0, CurrentValue: > 0 });
    }

    private static bool StockQuantityIsChangedTo0(EntityEntry<Product> trackedProductEntityEntry)
    {
        bool isQuantityInStockModified = trackedProductEntityEntry
            .Property(p => p.QuantityInStock)
            .IsModified;

        bool isQuantityInStockEqualTo0 = trackedProductEntityEntry
            .Property(p => p.QuantityInStock)
            .CurrentValue == 0;

        return isQuantityInStockModified && isQuantityInStockEqualTo0;
    }
}