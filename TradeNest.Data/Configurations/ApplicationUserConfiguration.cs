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
                UserName = "Haribo",
                NormalizedUserName = "HARIBO",
                Email = "Har1b0@gmail.com",
                NormalizedEmail = "HAR1B0@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(
                    new ApplicationUser() { UserName = "Har1b0@gmail.com" },
                    "Har1b0!")
            },
            new ApplicationUser()
            {
                Id = Guid.Parse("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                UserName = "Mirko",
                NormalizedUserName = "MIRKO",
                Email = "M1rk0@gmail.com",
                NormalizedEmail = "M1RK0@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(
                    new ApplicationUser() { UserName = "M1rk0@gmail.com" },
                    "M1rk0!")
            }
        };

        return applicationUsersToSeed;
    }
}