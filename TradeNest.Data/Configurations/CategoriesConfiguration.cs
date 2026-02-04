using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeNest.Data.Models;

namespace TradeNest.Data.Configurations;

public class CategoriesConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasData(this.SeedCategories());
    }

    private IEnumerable<Category> SeedCategories()
    {
        IEnumerable<Category> categoriesToSeed = new Category[]
        {
            new Category { Id = Guid.Parse("c6b3e6e0-3e3d-4c3d-8e7c-0b9a1b4e2f3g"), Name = "Electronics" },
            new Category { Id = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef"), Name = "Books" },
            new Category { Id = Guid.Parse("f0e9d8c7-b6a5-4321-fedc-ba9876543210"), Name = "Home & Garden" },
            new Category { Id = Guid.Parse("1a2b3c4d-5e6f-7890-abcd-ef0123456789"), Name = "Clothing" },
            new Category { Id = Guid.Parse("9f8e7d6c-5b4a-3210-fedc-ba9876543210"), Name = "Sporting Goods" }
        };

        return categoriesToSeed;
    }
}