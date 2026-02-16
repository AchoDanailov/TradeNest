using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeUsersNonNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_OwnerId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key referencing the product's owner primary key.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Foreign key referencing the product's owner primary key.");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key referencing the user making the order.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Foreign key referencing the user making the order.");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_OwnerId",
                table: "Products",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_OwnerId",
                table: "Products");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Foreign key referencing the product's owner primary key.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key referencing the product's owner primary key.");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Foreign key referencing the user making the order.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key referencing the user making the order.");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_OwnerId",
                table: "Products",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
