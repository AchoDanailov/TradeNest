using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"), 0, "c0166024-3922-4808-be4f-08163ebd2f5b", "M1rk0@gmail.com", true, false, null, "M1RK0@GMAIL.COM", "MIRKO", "AQAAAAIAAYagAAAAECLde3blD8CjKUqC6p/IOCbbpu7WP9+qhqLWC9/SLyNdELgDRQzGWpMprr0/I//0HA==", null, false, null, false, "Mirko" },
                    { new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"), 0, "9d6de701-75bb-4730-832a-2da1bc7dc436", "Har1b0@gmail.com", true, false, null, "HAR1B0@GMAIL.COM", "HARIBO", "AQAAAAIAAYagAAAAEGm+miPp1d1uQ4UpvxuhLUymHmalqK4arvxP4aI24lQf3JUMJelh2fyTnoBRDcmhOw==", null, false, null, false, "Haribo" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("1a2b3c4d-5e6f-7890-abcd-ef0123456789"), "Clothing" },
                    { new Guid("9f8e7d6c-5b4a-3210-fedc-ba9876543210"), "Sporting Goods" },
                    { new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"), "Books" },
                    { new Guid("c6b3e6e0-3e3d-4c3d-8e7c-0b9a1b4e2f30"), "Electronics" },
                    { new Guid("f0e9d8c7-b6a5-4321-fedc-ba9876543210"), "Home & Garden" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CostPrice", "CreatedOn", "Description", "IsDeleted", "IsEnabled", "Name", "OwnerId", "QuantityInStock", "SellingPrice" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"), new Guid("c6b3e6e0-3e3d-4c3d-8e7c-0b9a1b4e2f30"), 45.00m, new DateTime(2025, 12, 6, 18, 26, 4, 418, DateTimeKind.Utc).AddTicks(9623), "High-fidelity audio with noise-cancelling features and comfortable earcups for extended listening sessions. Up to 20 hours of battery life.", false, true, "Wireless Bluetooth Headphones", new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"), 10, 99.99m },
                    { new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"), new Guid("c6b3e6e0-3e3d-4c3d-8e7c-0b9a1b4e2f30"), 300.00m, new DateTime(2025, 11, 6, 18, 26, 4, 418, DateTimeKind.Utc).AddTicks(9657), "4K Ultra HD Smart TV with vibrant colors and intelligent processing Built-in streaming apps for endless entertainment. Includes a voice remote.", false, true, "Smart LED TV 55-inch", new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"), 5, 599.00m },
                    { new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"), new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"), 12.00m, new DateTime(2025, 12, 21, 18, 26, 4, 418, DateTimeKind.Utc).AddTicks(9662), "A comprehensive guide for beginners to learn C# programming language covering basics to advanced topics with practical examples and exercises.", false, true, "Introduction to C# Programming", new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"), 20, 24.99m },
                    { new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"), new Guid("f0e9d8c7-b6a5-4321-fedc-ba9876543210"), 80.00m, new DateTime(2025, 11, 21, 18, 26, 4, 418, DateTimeKind.Utc).AddTicks(9667), "Designed for maximum comfort and support during long working hours. Features adjustable lumbar support, armrests, and headrest.", false, true, "Ergonomic Office Chair", new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"), 10, 179.99m },
                    { new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"), new Guid("f0e9d8c7-b6a5-4321-fedc-ba9876543210"), 15.00m, new DateTime(2026, 1, 15, 18, 26, 4, 418, DateTimeKind.Utc).AddTicks(9672), "A beautiful collection of five low-maintenance succulent plants, perfect for decorating your home or office space. Comes with decorative pots.", false, true, "Indoor Plant Set - Succulents", new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"), 30, 34.99m }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Id", "IsFrontImage", "ProductId", "Url" },
                values: new object[,]
                {
                    { new Guid("032b49cb-f302-4a5d-8aab-68225304e43e"), true, new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"), "~/images/csharp_book.png" },
                    { new Guid("203a1575-d818-4ddc-97c0-05a850e53370"), false, new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"), "~/images/tv_part_2.png" },
                    { new Guid("25665742-f35f-4e0d-adba-c02f5eb7a444"), false, new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"), "~/images/chair_angle_2.png" },
                    { new Guid("3c265bcb-bf21-4951-9ddd-438934e9686d"), false, new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"), "~/images/tv_part_3.png" },
                    { new Guid("75b8d9c0-a895-4856-893b-a5102952e284"), false, new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"), "~/images/chair_angle_3.png" },
                    { new Guid("76384254-1606-4b50-86ae-cef4af3fb0aa"), true, new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"), "~/images/succulents.png" },
                    { new Guid("988c072b-dde1-43e3-bac1-04284a955fc6"), true, new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"), "~/images/tv.png" },
                    { new Guid("b42f4a9e-51a3-4c84-ba33-f74f0a48d544"), false, new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"), "~/images/tv_part_1.png" },
                    { new Guid("b6a043db-7cba-4997-8a37-f794dccf7c4b"), false, new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"), "~/images/headphones_part_1.png" },
                    { new Guid("b8dbe53a-fadc-4dfb-8b3a-e82f1334f052"), true, new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"), "~/images/chair_angle_1.png" },
                    { new Guid("ccfb563e-bca5-4f0e-a55d-61d20bdf789f"), false, new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"), "~/images/headphones_part_3.png" },
                    { new Guid("d7089810-b52b-4a91-80f7-b7f41de2045e"), false, new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"), "~/images/headphones_part_2.png" },
                    { new Guid("f32e52d9-9f9c-416e-8567-e1c5502a9b1a"), true, new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"), "~/images/headphones.png" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("1a2b3c4d-5e6f-7890-abcd-ef0123456789"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("9f8e7d6c-5b4a-3210-fedc-ba9876543210"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("032b49cb-f302-4a5d-8aab-68225304e43e"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("203a1575-d818-4ddc-97c0-05a850e53370"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("25665742-f35f-4e0d-adba-c02f5eb7a444"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("3c265bcb-bf21-4951-9ddd-438934e9686d"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("75b8d9c0-a895-4856-893b-a5102952e284"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("76384254-1606-4b50-86ae-cef4af3fb0aa"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("988c072b-dde1-43e3-bac1-04284a955fc6"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("b42f4a9e-51a3-4c84-ba33-f74f0a48d544"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("b6a043db-7cba-4997-8a37-f794dccf7c4b"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("b8dbe53a-fadc-4dfb-8b3a-e82f1334f052"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("ccfb563e-bca5-4f0e-a55d-61d20bdf789f"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("d7089810-b52b-4a91-80f7-b7f41de2045e"));

            migrationBuilder.DeleteData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("f32e52d9-9f9c-416e-8567-e1c5502a9b1a"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("c6b3e6e0-3e3d-4c3d-8e7c-0b9a1b4e2f30"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f0e9d8c7-b6a5-4321-fedc-ba9876543210"));
        }
    }
}
