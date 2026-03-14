using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TradeNest.Data.Models;
using static TradeNest.Data.Common.EntityModelsConstants.Product;

namespace TradeNest.Data.Configurations;

public class ProductsConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasOne(p => p.Owner)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(p => p.CreatedOn)
            .HasDefaultValueSql(DefaultValueForCreatedOnColumn);

        builder.Property(p => p.IsEnabled)
            .HasDefaultValue(DefaultValueForIsEnabledColumn);

        builder.HasQueryFilter(p => p.IsDeleted == false);

        builder.HasData(this.SeedProducts());
    }
    
    private IEnumerable<Product> SeedProducts()
    {
        Guid user1Id = Guid.Parse("d05a8fe7-cf0a-4895-89aa-9068c334ec1b");
        Guid user2Id = Guid.Parse("a8e18a83-adfb-4116-9e35-3be16446f9b8");
        
        Guid electronicsCategoryId = Guid.Parse("c6b3e6e0-3e3d-4c3d-8e7c-0b9a1b4e2f30");
        Guid booksCategoryId = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef");
        Guid homeGardenCategoryId = Guid.Parse("f0e9d8c7-b6a5-4321-fedc-ba9876543210");

        IEnumerable<Product> productsToSeed = new Product[]
        {
            new Product
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-1111-222233334444"),
                Name = "Wireless Bluetooth Headphones",
                Description = "High-fidelity audio with noise-cancelling features and comfortable earcups for extended listening sessions. Up to 20 hours of battery life.",
                QuantityInStock = 10,
                CostPrice = 45.00m,
                SellingPrice = 99.99m,
                CreatedOn = DateTime.UtcNow,
                IsEnabled = true,
                IsDeleted = false,
                OwnerId = user1Id,
                CategoryId = electronicsCategoryId
            },
            new Product
            {
                Id = Guid.Parse("b2c3d4e5-f6a7-8901-2222-333344445555"),
                Name = "Smart LED TV 55-inch",
                Description = "4K Ultra HD Smart TV with vibrant colors and intelligent processing Built-in streaming apps for endless entertainment. Includes a voice remote.",
                QuantityInStock = 5,
                CostPrice = 300.00m,
                SellingPrice = 599.00m,
                CreatedOn = DateTime.UtcNow,
                IsEnabled = true,
                IsDeleted = false,
                OwnerId = user1Id,
                CategoryId = electronicsCategoryId
            },
            new Product
            {
                Id = Guid.Parse("d4e5f6a7-b8c9-0123-4444-555566667777"),
                Name = "Introduction to C# Programming",
                Description = "A comprehensive guide for beginners to learn C# programming language covering basics to advanced topics with practical examples and exercises.",
                QuantityInStock = 20,
                CostPrice = 12.00m,
                SellingPrice = 24.99m,
                CreatedOn = DateTime.UtcNow,
                IsEnabled = true,
                IsDeleted = false,
                OwnerId = user2Id,
                CategoryId = booksCategoryId
            },
            new Product
            {
                Id = Guid.Parse("e5f6a7b8-c9d0-1234-5555-666677778888"),
                Name = "Ergonomic Office Chair",
                Description = "Designed for maximum comfort and support during long working hours. Features adjustable lumbar support, armrests, and headrest.",
                QuantityInStock = 10,
                CostPrice = 80.00m,
                SellingPrice = 179.99m,
                CreatedOn = DateTime.UtcNow.AddDays(-75),
                IsEnabled = true,
                IsDeleted = false,
                OwnerId = user2Id,
                CategoryId = homeGardenCategoryId
            },
            new Product
            {
                Id = Guid.Parse("f6a7b8c9-d0e1-2345-6666-777788889999"),
                Name = "Indoor Plant Set - Succulents",
                Description = "A beautiful collection of five low-maintenance succulent plants, perfect for decorating your home or office space. Comes with decorative pots.",
                QuantityInStock = 30,
                CostPrice = 15.00m,
                SellingPrice = 34.99m,
                CreatedOn = DateTime.UtcNow.AddDays(-20),
                IsEnabled = true,
                IsDeleted = false,
                OwnerId = user2Id,
                CategoryId = homeGardenCategoryId
            }
        };

        return productsToSeed;
    }       
}