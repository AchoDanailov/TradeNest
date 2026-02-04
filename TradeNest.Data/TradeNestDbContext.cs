using TradeNest.Data.Models;
using static TradeNest.Data.Utilities.DbContextOptimizationHelper;
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
        UpdateProductIsEnabledPropertyAsRequired(productsToModifyIsEnabledProperty);

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
    }
}