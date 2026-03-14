using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                },
                comment: "Holds User data.");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Category's primary key."),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Category's name.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                },
                comment: "Holds category data.");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Cart's primary key."),
                    CartOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "One to one relation with User. Dependant.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_AspNetUsers_CartOwnerId",
                        column: x => x.CartOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Holds Cart data.");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Order's primary key."),
                    SubmittedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "The date and time at which the order has been submitted."),
                    TotalPrice = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false, comment: "Holds the value of the order's total price when order is submitted."),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the user that has made the order.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Holds order's data.");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Product's primary key."),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Product's name."),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false, comment: "Product's description."),
                    QuantityInStock = table.Column<int>(type: "int", nullable: false, comment: "The quantity of the product that is available in stock."),
                    CostPrice = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: true, comment: "The price cost of attaining/producing the product. Is for user statistics. Nullable."),
                    SellingPrice = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false, comment: "The price the product is being sold at."),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "Date of creating. Has default universal time set on record insertion to date and time of insertion."),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Value is used to show weather the product is enabled or disabled for selling."),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, comment: "Value is used to show weather the product deleted."),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the product's owner primary key."),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the product's category primary key.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Holds product's data.");

            migrationBuilder.CreateTable(
                name: "CartsProducts",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the product's primary key."),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the cart's primary key."),
                    ProductQuantityAdded = table.Column<int>(type: "int", nullable: false, comment: "The value describes how much quantity of the given product is added in the given Cart."),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false, computedColumnSql: "GETUTCDATE()", comment: "The date and time that the product was added to the cart.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartsProducts", x => new { x.ProductId, x.CartId });
                    table.ForeignKey(
                        name: "FK_CartsProducts_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartsProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Mapping entity between Cart and Products - represents product added to cart.");

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Image's primary key"),
                    Url = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false, comment: "Image's Url"),
                    IsFrontImage = table.Column<bool>(type: "bit", nullable: false, comment: "Value represents weather the image is used as a front image for the product or not."),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the image's product primary key.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Holds Image data.");

            migrationBuilder.CreateTable(
                name: "OrdersProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "OrderProduct primary key."),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the Order's primary key."),
                    ProductNameAtOrderTime = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "The product name at the time that the order is submitted."),
                    OriginalProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the original product primary key."),
                    QuantityOrdered = table.Column<int>(type: "int", nullable: false, comment: "Represents the quantity of the product that was ordered."),
                    CostPriceAtOrderTime = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: true, comment: "Represents the cost price of the product at the moment the order is submitted."),
                    UnitSellingPriceAtOrderTime = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false, comment: "Represents the selling price of a unit of the product at the moment the order is submitted."),
                    TotalProductPriceAtOrderTime = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false, computedColumnSql: "[UnitSellingPriceAtOrderTime] * [QuantityOrdered]", stored: true, comment: "Computed and stored in a column from QuantityOrdered * UnitSellingPriceAtOrderTime.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdersProducts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdersProducts_Products_OriginalProductId",
                        column: x => x.OriginalProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Represents the product state at the moment of the order being submitted.");

            migrationBuilder.CreateTable(
                name: "UsersWatchlistsProducts",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the watchlist's owner primary key."),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key referencing the watchlist's product primary key.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersWatchlistsProducts", x => new { x.UserId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_UsersWatchlistsProducts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersWatchlistsProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Mapping entity representing a product in a user's watchlist.");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"), 0, "a210468d-afe9-41df-8dc8-a8e5d798f944", "User2@gmail.com", true, false, null, "USER2@GMAIL.COM", "USER2", "AQAAAAIAAYagAAAAEDrJGKPOW1QrGI2LkJ6V1it4vpnjKR0rCQNTnjbwfuT6ST7EF8/JRCg93RCBd0BffA==", null, false, "12fdb501-2dea-4628-960e-a795725799e4", false, "User2" },
                    { new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"), 0, "793361c8-6fb9-4f24-b9d9-a2026b65e404", "User1@gmail.com", true, false, null, "USER1@GMAIL.COM", "USER1", "AQAAAAIAAYagAAAAEEx+YuzqgSVINQhuAhYoRTM1S5pqJPtB0+Aom9H/FtdRdHhei2HxBrgpWkOcJBJqkg==", null, false, "a23d8472-0591-47d3-91ab-f6afcb3ee7fc", false, "User1" }
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
                    { new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"), new Guid("c6b3e6e0-3e3d-4c3d-8e7c-0b9a1b4e2f30"), 45.00m, new DateTime(2026, 3, 11, 6, 38, 57, 436, DateTimeKind.Utc).AddTicks(9577), "High-fidelity audio with noise-cancelling features and comfortable earcups for extended listening sessions. Up to 20 hours of battery life.", false, true, "Wireless Bluetooth Headphones", new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"), 10, 99.99m },
                    { new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"), new Guid("c6b3e6e0-3e3d-4c3d-8e7c-0b9a1b4e2f30"), 300.00m, new DateTime(2026, 3, 11, 6, 38, 57, 436, DateTimeKind.Utc).AddTicks(9592), "4K Ultra HD Smart TV with vibrant colors and intelligent processing Built-in streaming apps for endless entertainment. Includes a voice remote.", false, true, "Smart LED TV 55-inch", new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"), 5, 599.00m },
                    { new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"), new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef"), 12.00m, new DateTime(2026, 3, 11, 6, 38, 57, 436, DateTimeKind.Utc).AddTicks(9596), "A comprehensive guide for beginners to learn C# programming language covering basics to advanced topics with practical examples and exercises.", false, true, "Introduction to C# Programming", new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"), 20, 24.99m },
                    { new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"), new Guid("f0e9d8c7-b6a5-4321-fedc-ba9876543210"), 80.00m, new DateTime(2025, 12, 26, 6, 38, 57, 436, DateTimeKind.Utc).AddTicks(9599), "Designed for maximum comfort and support during long working hours. Features adjustable lumbar support, armrests, and headrest.", false, true, "Ergonomic Office Chair", new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"), 10, 179.99m },
                    { new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"), new Guid("f0e9d8c7-b6a5-4321-fedc-ba9876543210"), 15.00m, new DateTime(2026, 2, 19, 6, 38, 57, 436, DateTimeKind.Utc).AddTicks(9606), "A beautiful collection of five low-maintenance succulent plants, perfect for decorating your home or office space. Comes with decorative pots.", false, true, "Indoor Plant Set - Succulents", new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"), 30, 34.99m }
                });

            migrationBuilder.InsertData(
                table: "Images",
                columns: new[] { "Id", "IsFrontImage", "ProductId", "Url" },
                values: new object[,]
                {
                    { new Guid("032b49cb-f302-4a5d-8aab-68225304e43e"), true, new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"), "/images/products/csharp_book.png" },
                    { new Guid("203a1575-d818-4ddc-97c0-05a850e53370"), false, new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"), "/images/products/tv_part_2.png" },
                    { new Guid("25665742-f35f-4e0d-adba-c02f5eb7a444"), false, new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"), "/images/products/chair_angle_2.png" },
                    { new Guid("3c265bcb-bf21-4951-9ddd-438934e9686d"), false, new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"), "/images/products/tv_part_3.png" },
                    { new Guid("75b8d9c0-a895-4856-893b-a5102952e284"), false, new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"), "/images/products/chair_angle_3.png" },
                    { new Guid("76384254-1606-4b50-86ae-cef4af3fb0aa"), true, new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"), "/images/products/succulents.png" },
                    { new Guid("988c072b-dde1-43e3-bac1-04284a955fc6"), true, new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"), "/images/products/tv.png" },
                    { new Guid("b42f4a9e-51a3-4c84-ba33-f74f0a48d544"), false, new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"), "/images/products/tv_part_1.png" },
                    { new Guid("b6a043db-7cba-4997-8a37-f794dccf7c4b"), false, new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"), "/images/products/headphones_part_1.png" },
                    { new Guid("b8dbe53a-fadc-4dfb-8b3a-e82f1334f052"), true, new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"), "/images/products/chair_angle_1.png" },
                    { new Guid("ccfb563e-bca5-4f0e-a55d-61d20bdf789f"), false, new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"), "/images/products/headphones_part_3.png" },
                    { new Guid("d7089810-b52b-4a91-80f7-b7f41de2045e"), false, new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"), "/images/products/headphones_part_2.png" },
                    { new Guid("f32e52d9-9f9c-416e-8567-e1c5502a9b1a"), true, new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"), "/images/products/headphones.png" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CartOwnerId",
                table: "Carts",
                column: "CartOwnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartsProducts_CartId",
                table: "CartsProducts",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ProductId",
                table: "Images",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersProducts_OrderId",
                table: "OrdersProducts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdersProducts_OriginalProductId",
                table: "OrdersProducts",
                column: "OriginalProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OwnerId",
                table: "Products",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersWatchlistsProducts_ProductId",
                table: "UsersWatchlistsProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CartsProducts");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "OrdersProducts");

            migrationBuilder.DropTable(
                name: "UsersWatchlistsProducts");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
