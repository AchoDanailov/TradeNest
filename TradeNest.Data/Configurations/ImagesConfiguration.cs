using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradeNest.Data.Models;

namespace TradeNest.Data.Configurations;

public class ImagesConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasData(this.SeedImages());
    }

    private IEnumerable<Image> SeedImages()
    {
        Guid headphoneProductId = Guid.Parse("a1b2c3d4-e5f6-7890-1111-222233334444");
        Guid smartTvProductId = Guid.Parse("b2c3d4e5-f6a7-8901-2222-333344445555");
        Guid csharpBookId = Guid.Parse("d4e5f6a7-b8c9-0123-4444-555566667777");
        Guid officeChairProductId = Guid.Parse("e5f6a7b8-c9d0-1234-5555-666677778888");
        Guid succulentsProductId = Guid.Parse("f6a7b8c9-d0e1-2345-6666-777788889999");

        IEnumerable<Image> imagesToSeed = new Image[]
        {
            new Image
            {
                Id = Guid.Parse("b8dbe53a-fadc-4dfb-8b3a-e82f1334f052"),
                Url = "./images/chair_angle_1.png",
                IsFrontImage = true,
                ProductId = officeChairProductId
            },
            new Image
            {
                Id = Guid.Parse("25665742-f35f-4e0d-adba-c02f5eb7a444"),
                Url = "./images/chair_angle_2.png",
                IsFrontImage = false,
                ProductId = officeChairProductId
            },
            new Image
            {
                Id = Guid.Parse("75b8d9c0-a895-4856-893b-a5102952e284"),
                Url = "./images/chair_angle_3.png",
                IsFrontImage = false,
                ProductId = officeChairProductId
            },
            new Image
            {
                Id = Guid.Parse("f32e52d9-9f9c-416e-8567-e1c5502a9b1a"),
                Url = "./images/headphones.png",
                IsFrontImage = true,
                ProductId = headphoneProductId
            },
            new Image
            {
                Id = Guid.Parse("b6a043db-7cba-4997-8a37-f794dccf7c4b"),
                Url = "./images/headphones_part_1.png",
                IsFrontImage = false,
                ProductId = headphoneProductId
            },
            new Image
            {
                Id = Guid.Parse("d7089810-b52b-4a91-80f7-b7f41de2045e"),
                Url = "./images/headphones_part_2.png",
                IsFrontImage = false,
                ProductId = headphoneProductId
            },
            new Image
            {
                Id = Guid.Parse("ccfb563e-bca5-4f0e-a55d-61d20bdf789f"),
                Url = "./images/headphones_part_3.png",
                IsFrontImage = false,
                ProductId = headphoneProductId
            },
            new Image
            {
                Id = Guid.Parse("b42f4a9e-51a3-4c84-ba33-f74f0a48d544"),
                Url = "./images/tv_part_1.png",
                IsFrontImage = false,
                ProductId = smartTvProductId
            },
            new Image
            {
                Id = Guid.Parse("203a1575-d818-4ddc-97c0-05a850e53370"),
                Url = "./images/tv_part_2.png",
                IsFrontImage = false,
                ProductId = smartTvProductId
            },
            new Image
            {
                Id = Guid.Parse("3c265bcb-bf21-4951-9ddd-438934e9686d"),
                Url = "./images/tv_part_3.png",
                IsFrontImage = false,
                ProductId = smartTvProductId
            },
            new Image
            {
                Id = Guid.Parse("988c072b-dde1-43e3-bac1-04284a955fc6"),
                Url = "./images/tv.png",
                IsFrontImage = true,
                ProductId = smartTvProductId
            },
            new Image
            {
                Id = Guid.Parse("76384254-1606-4b50-86ae-cef4af3fb0aa"),
                Url = "./images/succulents.png",
                IsFrontImage = true,
                ProductId = succulentsProductId
            },
            new Image
            {
                Id = Guid.Parse("032b49cb-f302-4a5d-8aab-68225304e43e"),
                Url = "./images/csharp_book.png",
                IsFrontImage = true,
                ProductId = csharpBookId
            }
        };

        return imagesToSeed;
    }
}