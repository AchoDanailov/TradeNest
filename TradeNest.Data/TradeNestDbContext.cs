using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TradeNest.Data;

public class TradeNestDbContext : IdentityDbContext
{
    public TradeNestDbContext(DbContextOptions<TradeNestDbContext> options)
        : base(options)
    {
    }
}