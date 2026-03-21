using TradeNest.Services.Models.Category;

namespace TradeNest.Services.Core.Interfaces;

public interface ICategoriesService
{
    /// <summary>
    /// Determines whether a category with the specified identifier exists.
    /// </summary>
    /// <param name="id">The category identifier.</param>
    /// <returns>A task that returns true if the category exists; otherwise, false.</returns>
    Task<bool> CategoryExistsByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all categories ordered by name.
    /// </summary>
    /// <returns>A task that returns the collection of categories, empty if none exist.</returns>
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

    /// <summary>
    /// Retrieves all categories along with the front image of their best-selling product.
    /// </summary>
    /// <returns>
    /// A task that returns the collection of categories with best-seller image,
    /// null if none exist.
    /// </returns>
    Task<IEnumerable<CategoryWithBestSellerImageDto>> GetAllCategoriesWithBestSellerImageAsync();
}