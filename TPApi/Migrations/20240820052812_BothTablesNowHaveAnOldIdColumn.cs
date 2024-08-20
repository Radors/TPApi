using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPApi.Migrations
{
    /// <inheritdoc />
    public partial class BothTablesNowHaveAnOldIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "FoodProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "FoodEmbeddings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldId",
                table: "FoodProducts");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "FoodEmbeddings");
        }
    }
}
