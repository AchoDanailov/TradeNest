using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeUsersAndProductsDeletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersProducts_Products_OriginalProductId",
                table: "OrdersProducts");

            migrationBuilder.AlterColumn<Guid>(
                name: "OriginalProductId",
                table: "OrdersProducts",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Foreign key referencing the original product primary key.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Foreign key referencing the original product primary key.");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersProducts_Products_OriginalProductId",
                table: "OrdersProducts",
                column: "OriginalProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
            
            migrationBuilder.AddColumn<bool>(
                name: "PersonalInformationIsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersProducts_Products_OriginalProductId",
                table: "OrdersProducts");

            migrationBuilder.AlterColumn<Guid>(
                name: "OriginalProductId",
                table: "OrdersProducts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "Foreign key referencing the original product primary key.",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Foreign key referencing the original product primary key.");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersProducts_Products_OriginalProductId",
                table: "OrdersProducts",
                column: "OriginalProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "PersonalInformationIsDeleted",
                table: "AspNetUsers");
        }
    }
}
