using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeApplicationUserSeededData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "08b5ab58-c7d0-4d7c-b42a-14bdefcecfcd", "M1RK0@GMAIL.COM", "AQAAAAIAAYagAAAAEGeC0V79Ct6q678tAurl5l+yk1AF6rMC8LdYw7nDeSYIjjezhmNxhBN6PAddNenmEw==", "5b7e526e-fa2c-4a20-a048-44cb8f7bf726", "M1rk0@gmail.com" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "1a92bd59-8910-482e-b6f7-a0dc6fd0a491", "HAR1B0@GMAIL.COM", "AQAAAAIAAYagAAAAEJKSMTRIsaLKEJkTT5KUYjaRUwr19e44CU2JFbg/mURanaeG7btrf2FXouIBBJccDA==", "35a5eb26-b4cd-4c33-80fb-8b57deebd7f2", "Har1b0@gmail.com" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 6, 23, 35, 13, 156, DateTimeKind.Utc).AddTicks(1028));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 6, 23, 35, 13, 156, DateTimeKind.Utc).AddTicks(1055));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 21, 23, 35, 13, 156, DateTimeKind.Utc).AddTicks(1059));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 21, 23, 35, 13, 156, DateTimeKind.Utc).AddTicks(1062));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 15, 23, 35, 13, 156, DateTimeKind.Utc).AddTicks(1066));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "c0166024-3922-4808-be4f-08163ebd2f5b", "MIRKO", "AQAAAAIAAYagAAAAECLde3blD8CjKUqC6p/IOCbbpu7WP9+qhqLWC9/SLyNdELgDRQzGWpMprr0/I//0HA==", null, "Mirko" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "9d6de701-75bb-4730-832a-2da1bc7dc436", "HARIBO", "AQAAAAIAAYagAAAAEGm+miPp1d1uQ4UpvxuhLUymHmalqK4arvxP4aI24lQf3JUMJelh2fyTnoBRDcmhOw==", null, "Haribo" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 6, 18, 26, 4, 418, DateTimeKind.Utc).AddTicks(9623));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 6, 18, 26, 4, 418, DateTimeKind.Utc).AddTicks(9657));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 21, 18, 26, 4, 418, DateTimeKind.Utc).AddTicks(9662));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 21, 18, 26, 4, 418, DateTimeKind.Utc).AddTicks(9667));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 15, 18, 26, 4, 418, DateTimeKind.Utc).AddTicks(9672));
        }
    }
}
