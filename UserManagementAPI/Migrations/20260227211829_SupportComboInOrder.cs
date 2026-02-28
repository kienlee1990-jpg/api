using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastFoodAPI.Migrations
{
    /// <inheritdoc />
    public partial class SupportComboInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Foods_FoodId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "FoodId",
                table: "OrderItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ComboId",
                table: "OrderItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ComboId",
                table: "OrderItems",
                column: "ComboId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Combos_ComboId",
                table: "OrderItems",
                column: "ComboId",
                principalTable: "Combos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Foods_FoodId",
                table: "OrderItems",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Combos_ComboId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Foods_FoodId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_ComboId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ComboId",
                table: "OrderItems");

            migrationBuilder.AlterColumn<int>(
                name: "FoodId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Foods_FoodId",
                table: "OrderItems",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
