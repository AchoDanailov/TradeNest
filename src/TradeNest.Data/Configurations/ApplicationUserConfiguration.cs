using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TradeNest.Data.Models;

namespace TradeNest.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasData(this.SeedApplicationUser());
    }

    private IEnumerable<ApplicationUser> SeedApplicationUser()
    {
        IEnumerable<ApplicationUser> applicationUsersToSeed = new ApplicationUser[]
        {
            new ApplicationUser()
            {
                Id = Guid.Parse("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                UserName = "User1",
                NormalizedUserName = "USER1",
                Email = "User1@gmail.com",
                NormalizedEmail = "USER1@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>()
                    .HashPassword(new ApplicationUser() { UserName = "User1" }, "Password1"),
                SecurityStamp = Guid.NewGuid().ToString()
            },
            new ApplicationUser()
            {
                Id = Guid.Parse("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                UserName = "User2",
                NormalizedUserName = "USER2",
                Email = "User2@gmail.com",
                NormalizedEmail = "USER2@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>()
                    .HashPassword(new ApplicationUser() { UserName = "User2" }, "Password2"),
                SecurityStamp = Guid.NewGuid().ToString()
            }
        };

        return applicationUsersToSeed;
    }
}