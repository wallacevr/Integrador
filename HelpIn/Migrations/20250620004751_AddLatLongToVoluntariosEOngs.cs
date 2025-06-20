using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpIn.Migrations
{
    /// <inheritdoc />
    public partial class AddLatLongToVoluntariosEOngs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Voluntarios",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Voluntarios",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Ongs",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Ongs",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cep",
                table: "Ongs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Voluntarios");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Voluntarios");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Ongs");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Ongs");

            migrationBuilder.DropColumn(
                name: "cep",
                table: "Ongs");
        }
    }
}
