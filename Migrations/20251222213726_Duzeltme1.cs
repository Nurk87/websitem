using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyTodoList.Migrations
{
    /// <inheritdoc />
    public partial class Duzeltme1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sifre",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "KullaniciAdi",
                table: "Users",
                newName: "SecurityQuestion");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecurityAnswer",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SecurityAnswer",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "Sifre");

            migrationBuilder.RenameColumn(
                name: "SecurityQuestion",
                table: "Users",
                newName: "KullaniciAdi");
        }
    }
}
