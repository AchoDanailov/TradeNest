using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_OrdersSubmittedOnCol_OrdersProductsProductQuantityCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductsQuantity",
                table: "OrdersProducts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "The value describes how much quantity of the given product is added in the given order.");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedOn",
                table: "Orders",
                type: "datetime2",
                nullable: true,
                comment: "The date and time at which the order has been submitted.");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductsQuantity",
                table: "OrdersProducts");

            migrationBuilder.DropColumn(
                name: "SubmittedOn",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0e15ae70-beae-4983-b24a-5e9bf816f653", "AQAAAAIAAYagAAAAEI0rl39Z855Ruonsu9OdvHBx8DsHSkFCAJ8Vc5QlZGypUrrKDZdagPQGROK3EMmODw==", "71fb2efc-8c77-4558-8454-db2f7052f9e7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a15feb97-b9cb-4732-8b4d-4144ae3809eb", "AQAAAAIAAYagAAAAEHsdAPniMYUMzpATSa8XixiobxhBLONS8LXBwC9TiXgZNwryE9ZP5FVtLHRyHmXkDA==", "68ad5b85-4f91-4ecf-9694-fd21e0d672a9" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 18, 9, 45, 24, 38, DateTimeKind.Utc).AddTicks(1268));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 18, 9, 45, 24, 38, DateTimeKind.Utc).AddTicks(1294));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 2, 9, 45, 24, 38, DateTimeKind.Utc).AddTicks(1297));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 3, 9, 45, 24, 38, DateTimeKind.Utc).AddTicks(1305));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 27, 9, 45, 24, 38, DateTimeKind.Utc).AddTicks(1308));
        }
    }
}
