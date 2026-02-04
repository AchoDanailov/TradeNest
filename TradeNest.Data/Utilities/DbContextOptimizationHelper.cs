using Microsoft.EntityFrameworkCore.ChangeTracking;
using TradeNest.Data.Models;

namespace TradeNest.Data.Utilities;

public static class DbContextOptimizationHelpers
{
    /// <summary>
    /// Check
    /// </summary>
    /// <param name="productsToModifyIsEnabledProperty"></param>
    public static void UpdateProductIsEnabledPropertyAsRequired(
        IEnumerable<EntityEntry<Product>> productsToModifyIsEnabledProperty)
    {
        foreach (EntityEntry<Product> productEntityEntry in productsToModifyIsEnabledProperty)
        {
            if (StockQuantityIsChangedTo0(productEntityEntry))
                productEntityEntry.Property(p => p.IsEnabled).CurrentValue = false;
            else
                productEntityEntry.Property(p => p.IsEnabled).CurrentValue = true;
        }
    }
    
    /// <summary>
    /// The method checks weather the product's entity quantityInStock property value has been modified from 0 (to some other number) or to 0 (from some other number).
    /// </summary>
    /// <param name="trackedProductEntityEntry">The EntityEntry type for the tracked product (provides access to change tracking information and operations).</param>
    /// <returns> bool - indicating weather the corresponding product record quantityInStock value has been modified to or from 0.</returns>
    public static bool IsEnabledStatusShouldBeChanged(EntityEntry<Product> trackedProductEntityEntry)
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
    private static bool StockQuantityIsChangedTo0(EntityEntry<Product> trackedProductEntityEntry)
    {
        PropertyEntry<Product, int> stockQuantityPropertyEntry = trackedProductEntityEntry.Property(p => p.QuantityInStock);
        bool isStockQuantityModified = stockQuantityPropertyEntry.IsModified;

        return isStockQuantityModified && stockQuantityPropertyEntry.CurrentValue == 0;
    }
}