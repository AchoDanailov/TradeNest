using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IUsersRepository : IReadRepository<ApplicationUser>
{
    Task<bool> AddAsync(ApplicationUser user, string password);
}