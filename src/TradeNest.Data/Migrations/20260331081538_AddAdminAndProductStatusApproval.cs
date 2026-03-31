using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAndProductStatusApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdminId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true,
                comment: "The foreign key to the admin entity that has the product listed for approval.");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Value representing weather the product has been approved or not or is still waiting for approval.");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The primary key of the Admin entity."),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the user that is an admin. 1 to 1.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Entity representing an application admin. 1 to 1 with User.");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"),
                columns: new[] { "AdminId", "ApprovalStatus" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"),
                columns: new[] { "AdminId", "ApprovalStatus" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"),
                columns: new[] { "AdminId", "ApprovalStatus" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"),
                columns: new[] { "AdminId", "ApprovalStatus" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"),
                columns: new[] { "AdminId", "ApprovalStatus" },
                values: new object[] { null, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Products_AdminId",
                table: "Products",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Admins_AdminId",
                table: "Products",
                column: "AdminId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Admins_AdminId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropIndex(
                name: "IX_Products_AdminId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3e00c32f-ed63-454c-b9d8-1e2989e239bc", "AQAAAAIAAYagAAAAEEKyanlAmzzaMVnskw1TS+E6AYGiYBbVNqeaBbWmSYeJ6vvqt2A/gyeHDO+Db0k2rQ==", "d439b3a5-ad3e-4a82-b4af-eb20ec8a634f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "542090ca-d27b-43c5-9907-6d779fd107a0", "AQAAAAIAAYagAAAAEJaV+FYQ7PgQ7oMDgCnAtaSTn/4eoMzpCIGlzbTjY9YPLdDYlSjmFgKcxwekLjAueg==", "4ead1fee-504b-4391-8665-d134399df7dc" });
        }
    }
}
