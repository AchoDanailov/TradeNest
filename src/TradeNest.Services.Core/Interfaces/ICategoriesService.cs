using TradeNest.GCommon.Exceptions;
using TradeNest.Services.Models.Category;

namespace TradeNest.Services.Core.Interfaces;

public interface ICategoriesService
{
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

    /// <summary>
    /// Attempts to delete the category with the provided <paramref name="categoryId"/> identifier.
    /// </summary>
    /// <param name="userId">The identifier of the user attempting the operation.</param>
    /// <param name="categoryId">The identifier of the category to delete.</param>
    /// <returns>
    /// Task holding the result of the operation in a <see cref="DeleteCategoryResultDto"/> object.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if either of the provided <paramref name="categoryId"/> or <paramref name="userId"/>
    /// are with value <see cref="Guid.Empty"/>.
    /// </exception>
    /// <exception cref="ResourceNotFoundException">
    /// Thrown a category with the provided <paramref name="categoryId"/> was not found.
    /// </exception>
    /// <exception cref="UnauthorizedOperationException">
    /// Thrown if the user with the provided <paramref name="userId"/> is not an administrator.
    /// </exception>
    /// <exception cref="DataPersistException">
    /// Thrown if the data is not successfully persisted.
    /// </exception>
    Task<DeleteCategoryResultDto> DeleteCategoryByIdAsync(Guid userId, Guid categoryId);
}