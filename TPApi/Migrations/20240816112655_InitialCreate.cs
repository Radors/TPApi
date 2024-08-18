using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodEmbeddings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vector = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodEmbeddings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoodProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jod = table.Column<double>(type: "float", nullable: false),
                    Jarn = table.Column<double>(type: "float", nullable: false),
                    Kalcium = table.Column<double>(type: "float", nullable: false),
                    Kalium = table.Column<double>(type: "float", nullable: false),
                    Magnesium = table.Column<double>(type: "float", nullable: false),
                    Selen = table.Column<double>(type: "float", nullable: false),
                    Zink = table.Column<double>(type: "float", nullable: false),
                    A = table.Column<double>(type: "float", nullable: false),
                    B1 = table.Column<double>(type: "float", nullable: false),
                    B2 = table.Column<double>(type: "float", nullable: false),
                    B3 = table.Column<double>(type: "float", nullable: false),
                    B6 = table.Column<double>(type: "float", nullable: false),
                    B9 = table.Column<double>(type: "float", nullable: false),
                    B12 = table.Column<double>(type: "float", nullable: false),
                    C = table.Column<double>(type: "float", nullable: false),
                    D = table.Column<double>(type: "float", nullable: false),
                    E = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodProducts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodEmbeddings");

            migrationBuilder.DropTable(
                name: "FoodProducts");
        }
    }
}
