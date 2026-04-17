using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IAdminsRepository : IReadRepository<Admin>
{
    Task<Admin?> GetAdminByUserId(Guid userId);
    
    Task<bool> IsUserAdminByUserIdAsync(Guid userId);
    
    Task<bool> IsUserAdminAsync(ApplicationUser user);
    
    Task<bool> AddAsync(Admin admin);

    Task<bool> AddRangeAsync(IEnumerable<Admin> admins);
}