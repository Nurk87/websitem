using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTodoList.Migrations
{
    /// <inheritdoc />
    public partial class TabloGuncelleme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SahipAdi",
                table: "Todos");

            migrationBuilder.RenameColumn(
                name: "TamamlandiMi",
                table: "Todos",
                newName: "IsCompleted");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Todos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Todos");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "Todos",
                newName: "TamamlandiMi");

            migrationBuilder.AddColumn<string>(
                name: "SahipAdi",
                table: "Todos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
