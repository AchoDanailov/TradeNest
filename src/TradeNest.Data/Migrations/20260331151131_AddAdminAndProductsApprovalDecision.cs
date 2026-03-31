using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAndProductsApprovalDecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalDecisionMakerId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Foreign key to the admin that has taken a decision on the product approval if any.");

            migrationBuilder.AddColumn<int>(
                name: "ApprovalDecision_ApprovalStatus",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Value representing weather that product has been approved or not or is still waiting for a decision.");

            migrationBuilder.AddColumn<string>(
                name: "ApprovalDecision_DecisionJustification",
                table: "Products",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: true,
                comment: "The justification for the taken decision on the product approval.");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDecision_LastUpdatedOn",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                comment: "Value representing the time the ticket has been created.");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDecision_TimeOfDecision",
                table: "Products",
                type: "datetime2",
                nullable: true,
                comment: "Value representing the time the ticket has been processed and assigned approval status.");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Primary key of the admin entity."),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Foreign key to the user that is an Admin.")
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
                comment: "Represents the Admin entity.");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-1111-222233334444"),
                columns: new[] { "ApprovalDecisionMakerId" },
                values: new object[] { null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("b2c3d4e5-f6a7-8901-2222-333344445555"),
                columns: new[] { "ApprovalDecisionMakerId" },
                values: new object[] { null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("d4e5f6a7-b8c9-0123-4444-555566667777"),
                columns: new[] { "ApprovalDecisionMakerId" },
                values: new object[] { null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-1234-5555-666677778888"),
                columns: new[] { "ApprovalDecisionMakerId" },
                values: new object[] { null });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("f6a7b8c9-d0e1-2345-6666-777788889999"),
                columns: new[] { "ApprovalDecisionMakerId" },
                values: new object[] { null });

            migrationBuilder.CreateIndex(
                name: "IX_Products_ApprovalDecisionMakerId",
                table: "Products",
                column: "ApprovalDecisionMakerId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Admins_ApprovalDecisionMakerId",
                table: "Products",
                column: "ApprovalDecisionMakerId",
                principalTable: "Admins",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Admins_ApprovalDecisionMakerId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropIndex(
                name: "IX_Products_ApprovalDecisionMakerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApprovalDecisionMakerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApprovalDecision_ApprovalStatus",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApprovalDecision_DecisionJustification",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApprovalDecision_LastUpdatedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ApprovalDecision_TimeOfDecision",
                table: "Products");
        }
    }
}
