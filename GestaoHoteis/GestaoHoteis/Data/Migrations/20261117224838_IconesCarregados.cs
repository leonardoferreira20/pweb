using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoHoteis.Data.Migrations
{
    /// <inheritdoc />
    public partial class IconesCarregados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "IconeImagem",
                table: "RegimesAlimentares",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "IconeImagem",
                table: "Categorias",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "IconeImagem",
                table: "Caracteristicas",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconeImagem",
                table: "RegimesAlimentares");

            migrationBuilder.DropColumn(
                name: "IconeImagem",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "IconeImagem",
                table: "Caracteristicas");
        }
    }
}
