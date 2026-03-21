using TradeNest.Data.Models;
using TradeNest.Data.Repository.Interfaces;

namespace TradeNest.Data.Repository;

public class UsersRepository : BaseRepository<ApplicationUser>, IUsersRepository
{
    public UsersRepository(TradeNestDbContext dbContext) 
        : base(dbContext)
    {
    }
}