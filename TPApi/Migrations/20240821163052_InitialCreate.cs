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
                    OldId = table.Column<int>(type: "int", nullable: false),
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
                    OldId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Jod = table.Column<float>(type: "real", nullable: false),
                    Jarn = table.Column<float>(type: "real", nullable: false),
                    Kalcium = table.Column<float>(type: "real", nullable: false),
                    Kalium = table.Column<float>(type: "real", nullable: false),
                    Magnesium = table.Column<float>(type: "real", nullable: false),
                    Selen = table.Column<float>(type: "real", nullable: false),
                    Zink = table.Column<float>(type: "real", nullable: false),
                    A = table.Column<float>(type: "real", nullable: false),
                    B1 = table.Column<float>(type: "real", nullable: false),
                    B2 = table.Column<float>(type: "real", nullable: false),
                    B3 = table.Column<float>(type: "real", nullable: false),
                    B6 = table.Column<float>(type: "real", nullable: false),
                    B9 = table.Column<float>(type: "real", nullable: false),
                    B12 = table.Column<float>(type: "real", nullable: false),
                    C = table.Column<float>(type: "real", nullable: false),
                    D = table.Column<float>(type: "real", nullable: false),
                    E = table.Column<float>(type: "real", nullable: false)
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
