using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTodoList.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SahipAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YapilacakIs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Saat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TamamlandiMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Todos");
        }
    }
}
