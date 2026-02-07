using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeNest.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangePathsOfImageSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("032b49cb-f302-4a5d-8aab-68225304e43e"),
                column: "Url",
                value: "/images/products/csharp_book.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("203a1575-d818-4ddc-97c0-05a850e53370"),
                column: "Url",
                value: "/images/products/tv_part_2.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("25665742-f35f-4e0d-adba-c02f5eb7a444"),
                column: "Url",
                value: "/images/products/chair_angle_2.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("3c265bcb-bf21-4951-9ddd-438934e9686d"),
                column: "Url",
                value: "/images/products/tv_part_3.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("75b8d9c0-a895-4856-893b-a5102952e284"),
                column: "Url",
                value: "/images/products/chair_angle_3.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("76384254-1606-4b50-86ae-cef4af3fb0aa"),
                column: "Url",
                value: "/images/products/succulents.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("988c072b-dde1-43e3-bac1-04284a955fc6"),
                column: "Url",
                value: "/images/products/tv.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("b42f4a9e-51a3-4c84-ba33-f74f0a48d544"),
                column: "Url",
                value: "/images/products/tv_part_1.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("b6a043db-7cba-4997-8a37-f794dccf7c4b"),
                column: "Url",
                value: "/images/products/headphones_part_1.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("b8dbe53a-fadc-4dfb-8b3a-e82f1334f052"),
                column: "Url",
                value: "/images/products/chair_angle_1.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("ccfb563e-bca5-4f0e-a55d-61d20bdf789f"),
                column: "Url",
                value: "/images/products/headphones_part_3.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("d7089810-b52b-4a91-80f7-b7f41de2045e"),
                column: "Url",
                value: "/images/products/headphones_part_2.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("f32e52d9-9f9c-416e-8567-e1c5502a9b1a"),
                column: "Url",
                value: "/images/products/headphones.png");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("a8e18a83-adfb-4116-9e35-3be16446f9b8"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "08b5ab58-c7d0-4d7c-b42a-14bdefcecfcd", "AQAAAAIAAYagAAAAEGeC0V79Ct6q678tAurl5l+yk1AF6rMC8LdYw7nDeSYIjjezhmNxhBN6PAddNenmEw==", "5b7e526e-fa2c-4a20-a048-44cb8f7bf726" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("d05a8fe7-cf0a-4895-89aa-9068c334ec1b"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1a92bd59-8910-482e-b6f7-a0dc6fd0a491", "AQAAAAIAAYagAAAAEJKSMTRIsaLKEJkTT5KUYjaRUwr19e44CU2JFbg/mURanaeG7btrf2FXouIBBJccDA==", "35a5eb26-b4cd-4c33-80fb-8b57deebd7f2" });

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("032b49cb-f302-4a5d-8aab-68225304e43e"),
                column: "Url",
                value: "~/images/csharp_book.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("203a1575-d818-4ddc-97c0-05a850e53370"),
                column: "Url",
                value: "~/images/tv_part_2.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("25665742-f35f-4e0d-adba-c02f5eb7a444"),
                column: "Url",
                value: "~/images/chair_angle_2.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("3c265bcb-bf21-4951-9ddd-438934e9686d"),
                column: "Url",
                value: "~/images/tv_part_3.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("75b8d9c0-a895-4856-893b-a5102952e284"),
                column: "Url",
                value: "~/images/chair_angle_3.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("76384254-1606-4b50-86ae-cef4af3fb0aa"),
                column: "Url",
                value: "~/images/succulents.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("988c072b-dde1-43e3-bac1-04284a955fc6"),
                column: "Url",
                value: "~/images/tv.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("b42f4a9e-51a3-4c84-ba33-f74f0a48d544"),
                column: "Url",
                value: "~/images/tv_part_1.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("b6a043db-7cba-4997-8a37-f794dccf7c4b"),
                column: "Url",
                value: "~/images/headphones_part_1.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("b8dbe53a-fadc-4dfb-8b3a-e82f1334f052"),
                column: "Url",
                value: "~/images/chair_angle_1.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("ccfb563e-bca5-4f0e-a55d-61d20bdf789f"),
                column: "Url",
                value: "~/images/headphones_part_3.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("d7089810-b52b-4a91-80f7-b7f41de2045e"),
                column: "Url",
                value: "~/images/headphones_part_2.png");

            migrationBuilder.UpdateData(
                table: "Images",
                keyColumn: "Id",
                keyValue: new Guid("f32e52d9-9f9c-416e-8567-e1c5502a9b1a"),
                column: "Url",
                value: "~/images/headphones.png");

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
    }
}
