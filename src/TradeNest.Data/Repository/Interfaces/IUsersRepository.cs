using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IUsersRepository : IReadRepository<ApplicationUser>
{
    Task<IEnumerable<ApplicationUser>>
        GetAllUsersWithTheirRolesAsync(bool asReadOnly = false);

    Task<IEnumerable<ApplicationRole>> GetAllRolesAsync(bool asReadOnly = false);

    Task<bool> ExistsByIdWithForgottenIncludedAsync(Guid userId, bool asReadOnly = false);

    Task<ApplicationUser?> FindByIdWithForgottenIncludedAsync(Guid userId);
    
    Task<bool> AddAsync(ApplicationUser user, string password);

    Task DeleteAsync(ApplicationUser applicationUser);
}