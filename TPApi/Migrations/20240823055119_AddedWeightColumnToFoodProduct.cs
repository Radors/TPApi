using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedWeightColumnToFoodProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "FoodProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "FoodProducts");
        }
    }
}
