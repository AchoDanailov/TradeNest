using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetIsDeletedQueryFilterDirectionToFilterOutFalse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fba822d5-854b-4c4f-9504-4b2078356ec2", "AQAAAAIAAYagAAAAEAs1/hYkKytOQ8EDQEwSHZX3cfwGMj8+BpYHoZIHjWLfRTgIjEEYCRRjEuvctiSXnA==", "d80a6188-9aa4-4650-b688-b2c1df8860f7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8dccac47-0275-4e5f-85da-82ebca8b6ae1", "AQAAAAIAAYagAAAAEPkyCxRwmAHN6hlpBMwqUhpQAY3WpVaeGApbRAUYu9U7jnbQmYk7lARzFRb0+spGIg==", "4dce6c39-21ec-48b2-b41d-e29cb6b0331a" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 10, 13, 30, 58, 603, DateTimeKind.Utc).AddTicks(6881));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 10, 13, 30, 58, 603, DateTimeKind.Utc).AddTicks(6891));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 25, 13, 30, 58, 603, DateTimeKind.Utc).AddTicks(6894));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 25, 13, 30, 58, 603, DateTimeKind.Utc).AddTicks(6897));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 19, 13, 30, 58, 603, DateTimeKind.Utc).AddTicks(6902));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b9e0c3e3-c718-44cd-b792-ced30304b309", "AQAAAAIAAYagAAAAEP3Q/NahVSi/qUD8tlC7g/v69BHdHodGu3A9xNUs27UH9vaQuKqUW+6UBdBj+D5mFw==", "57d04621-16ae-4bd2-9d90-6e0ff229f874" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "29ec372d-fd6b-430c-93ba-8b28dad8ec4e", "AQAAAAIAAYagAAAAEKi6USO1Qz3eY0T4qnucAWf+CZWbJFwpWA+M5xxrAhaKpUD44sGwDB5k770xP1PcHA==", "cb260b14-065c-4672-b1b6-1a4c382e2edc" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 9, 10, 21, 6, 163, DateTimeKind.Utc).AddTicks(442));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 9, 10, 21, 6, 163, DateTimeKind.Utc).AddTicks(457));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"),
                column: "CreatedOn",
                value: new DateTime(2025, 12, 24, 10, 21, 6, 163, DateTimeKind.Utc).AddTicks(460));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"),
                column: "CreatedOn",
                value: new DateTime(2025, 11, 24, 10, 21, 6, 163, DateTimeKind.Utc).AddTicks(464));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"),
                column: "CreatedOn",
                value: new DateTime(2026, 1, 18, 10, 21, 6, 163, DateTimeKind.Utc).AddTicks(467));
        }
    }
}
