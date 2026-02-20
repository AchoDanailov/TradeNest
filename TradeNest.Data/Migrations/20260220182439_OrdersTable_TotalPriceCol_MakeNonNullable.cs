using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrdersTable_TotalPriceCol_MakeNonNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Orders",
                type: "DECIMAL(10,2)",
                nullable: false,
                defaultValue: 0m,
                comment: "Holds the value of the order's total price when order is submitted.",
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,2)",
                oldNullable: true,
                oldComment: "Holds the value of the order's total price when order is submitted.");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "54d7e393-0863-4ca1-84f8-2b7fbeb51697", "AQAAAAIAAYagAAAAEMqOBlju2jqS2kb3jOKLWE8rBrDkozssb6fMyG5s3eUKLvSLCQ0Hw492wJkOgcF8Nw==", "02923b0c-63dc-469f-9b18-e44011108257" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "58102b47-763f-448a-b41e-173bf08f4073", "AQAAAAIAAYagAAAAEO4yROVxIYVqgJRDBxKkP+O37lqke6o25RZgfc903d1xXWlcf7t36pp58oZgeNSgaQ==", "a1123666-d774-41fe-ab80-fd5b080c13d0" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 22, 18, 24, 38, 662, DateTimeKind.Utc).AddTicks(8426));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 22, 18, 24, 38, 662, DateTimeKind.Utc).AddTicks(8514));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 6, 18, 24, 38, 662, DateTimeKind.Utc).AddTicks(8519));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 7, 18, 24, 38, 662, DateTimeKind.Utc).AddTicks(8523));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 31, 18, 24, 38, 662, DateTimeKind.Utc).AddTicks(8526));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "Orders",
                type: "DECIMAL(10,2)",
                nullable: true,
                comment: "Holds the value of the order's total price when order is submitted.",
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,2)",
                oldComment: "Holds the value of the order's total price when order is submitted.");

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
        }
    }
}
