using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastFoodAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Foods_FoodId",
                table: "CartItems");

            migrationBuilder.AlterColumn<int>(
                name: "FoodId",
                table: "CartItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ComboId",
                table: "CartItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ComboId",
                table: "CartItems",
                column: "ComboId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Combos_ComboId",
                table: "CartItems",
                column: "ComboId",
                principalTable: "Combos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Foods_FoodId",
                table: "CartItems",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Combos_ComboId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Foods_FoodId",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_ComboId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "ComboId",
                table: "CartItems");

            migrationBuilder.AlterColumn<int>(
                name: "FoodId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Foods_FoodId",
                table: "CartItems",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
