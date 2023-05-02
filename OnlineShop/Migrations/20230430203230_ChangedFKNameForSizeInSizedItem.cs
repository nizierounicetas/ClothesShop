using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShop.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFKNameForSizeInSizedItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SizedItems_Sizes_ItemId",
                table: "SizedItems");

            migrationBuilder.CreateIndex(
                name: "IX_SizedItems_SizeId",
                table: "SizedItems",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SizedItems_Sizes_SizeId",
                table: "SizedItems",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SizedItems_Sizes_SizeId",
                table: "SizedItems");

            migrationBuilder.DropIndex(
                name: "IX_SizedItems_SizeId",
                table: "SizedItems");

            migrationBuilder.AddForeignKey(
                name: "FK_SizedItems_Sizes_ItemId",
                table: "SizedItems",
                column: "ItemId",
                principalTable: "Sizes",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
