using TradeNest.Data.Models;

namespace TradeNest.Data.Repository.Interfaces;

public interface IAdminsRepository : IReadRepository<Admin>
{
    Task<bool> AddAsync(Admin admin);

    Task<bool> AddRangeAsync(IEnumerable<Admin> admins);
}