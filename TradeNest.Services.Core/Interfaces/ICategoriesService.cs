using TradeNest.Web.ViewModels.Category;

namespace TradeNest.Services.Core.Interfaces;

public interface ICategoriesService
{
    /// <summary>
    /// Checks if the category exists by the given id parameter's value
    /// </summary>
    /// <param name="id">The value used to find a match with.</param>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task result contains a value representing weather the category with the
    /// given id exists.
    /// </returns>
    /// <remarks>This method makes a connection to the given data store.</remarks>
    Task<bool> CategoryExists(Guid id);

    /// <summary>
    /// Provides a collection with all AllCategoriesViewModels ordered by category's name
    /// </summary>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The Task result contains a AllCategoriesViewModel collection.
    /// </returns>
    /// <remarks>If no categories are found the method returns an empty collection.</remarks>
    Task<IEnumerable<AllCategoriesViewModel>> GetAllCategoriesViewModels();
}