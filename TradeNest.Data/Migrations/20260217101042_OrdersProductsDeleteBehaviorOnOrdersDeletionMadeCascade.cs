using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrdersProductsDeleteBehaviorOnOrdersDeletionMadeCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersProducts_Orders_OrderId",
                table: "OrdersProducts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "43a50aff-ed22-43c6-bfc3-94f08518a6be", "AQAAAAIAAYagAAAAEATi8XzNk1XCGvR+jTzePL7Xbv/ulExug7ndomfRmLSIzWmjq/41ctxyd4G8cgJU/Q==", "80a37712-b4f8-481f-9ff2-073ca971117f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "89326aae-d195-4433-970e-f956e25d915c", "AQAAAAIAAYagAAAAECrZ3CK0zGY82i8lmUrDTUf7kU2rI0mOjbJRhdEpqqsdpjjhRqFei+mrJuaA2F1e/A==", "4b6a3267-e860-48a1-8cfc-eded2b35c7f8" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 19, 10, 10, 41, 891, DateTimeKind.Utc).AddTicks(1182));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 19, 10, 10, 41, 891, DateTimeKind.Utc).AddTicks(1192));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 3, 10, 10, 41, 891, DateTimeKind.Utc).AddTicks(1195));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 4, 10, 10, 41, 891, DateTimeKind.Utc).AddTicks(1198));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 28, 10, 10, 41, 891, DateTimeKind.Utc).AddTicks(1201));

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersProducts_Orders_OrderId",
                table: "OrdersProducts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersProducts_Orders_OrderId",
                table: "OrdersProducts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "77c6c89f-eb13-4182-8446-c30cb2a62cc8", "AQAAAAIAAYagAAAAEPJFHneUDj+DFdMEhiauHk49OcfXOHIqR2XInVGww3UyvKpGhv+rU1x8uydpybllbw==", "42ab713b-07a7-4a92-98b4-f94e7b3ca984" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "225b6cc7-fdcc-4c54-b1a4-2e01ed222716", "AQAAAAIAAYagAAAAEDltuUrlJsnTeNuYr8wTHKuNonVxA8EDyoetxTk9NRfTJ4/urAwTbwgCtHUJZIfDqQ==", "ae5e788e-fdaf-4797-92d9-2c4f011e7355" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 18, 15, 50, 0, 471, DateTimeKind.Utc).AddTicks(2464));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 18, 15, 50, 0, 471, DateTimeKind.Utc).AddTicks(2488));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 2, 15, 50, 0, 471, DateTimeKind.Utc).AddTicks(2492));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 3, 15, 50, 0, 471, DateTimeKind.Utc).AddTicks(2495));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 27, 15, 50, 0, 471, DateTimeKind.Utc).AddTicks(2498));

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersProducts_Orders_OrderId",
                table: "OrdersProducts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
