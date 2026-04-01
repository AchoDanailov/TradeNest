using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class AdminsRepository : BaseReadRepository<Admin>, IAdminsRepository
{
    public AdminsRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }

    public async Task<bool> AddAsync(Admin admin)
    {
        await this.DbContext.Admins.AddAsync(admin);
        int res = await this.DbContext.SaveChangesAsync();

        return res >= 1;
    }

    public async Task<bool> AddRangeAsync(IEnumerable<Admin> admins)
    {
        await this.DbContext.Admins.AddRangeAsync(admins);
        int res = await this.DbContext.SaveChangesAsync();

        return res >= 1;
    }
}