using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpIn.Migrations
{
    /// <inheritdoc />
    public partial class AddEnderecoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Ongs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");


            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Ongs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Logradouro",
                table: "Ongs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Ongs");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Ongs");

            migrationBuilder.DropColumn(
                name: "Logradouro",
                table: "Ongs");
        }
    }
}
