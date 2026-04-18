using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface ICategoriesRepository : IReadRepository<Category>
{
    Task<bool> AddAsync(Category cateogory);

    Task<bool> AddRangeAsync(IEnumerable<Category> categories);

    Task<bool> DeleteCategoryAsync(Category category);
}