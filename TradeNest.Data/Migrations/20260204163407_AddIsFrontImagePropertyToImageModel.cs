using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFrontImagePropertyToImageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFrontImage",
                table: "Images",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Value represents weather the image is used as a front image for the product or not.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFrontImage",
                table: "Images");
        }
    }
}
