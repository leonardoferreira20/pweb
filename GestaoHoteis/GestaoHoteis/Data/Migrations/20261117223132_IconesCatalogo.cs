using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoHoteis.Data.Migrations
{
    /// <inheritdoc />
    public partial class IconesCatalogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icone",
                table: "RegimesAlimentares",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icone",
                table: "Categorias",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icone",
                table: "Caracteristicas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // Escrito à mão (extra do grupo): numa base de dados que já tem os dados de exemplo,
            // as colunas novas ficam a NULL. Estes UPDATE dão o ícone certo a cada item de exemplo,
            // pelo nome. Numa base de dados nova não fazem nada (as tabelas ainda estão vazias):
            // aí é a classe Inicializacao que cria os itens já com o ícone.
            migrationBuilder.Sql("""
                UPDATE Categorias SET Icone = 'quarto-individual' WHERE Icone IS NULL AND Nome = N'Quarto Individual';
                UPDATE Categorias SET Icone = 'quarto-duplo' WHERE Icone IS NULL AND Nome = N'Quarto Duplo';
                UPDATE Categorias SET Icone = 'quarto-familiar' WHERE Icone IS NULL AND Nome = N'Quarto Familiar';
                UPDATE Categorias SET Icone = 'suite' WHERE Icone IS NULL AND Nome = N'Suite';
                UPDATE Categorias SET Icone = 'apartamento' WHERE Icone IS NULL AND Nome LIKE N'Apartamento%';
                UPDATE Caracteristicas SET Icone = 'wifi' WHERE Icone IS NULL AND Nome = N'Wi-Fi';
                UPDATE Caracteristicas SET Icone = 'ar-condicionado' WHERE Icone IS NULL AND Nome = N'Ar condicionado';
                UPDATE Caracteristicas SET Icone = 'varanda' WHERE Icone IS NULL AND Nome = N'Varanda';
                UPDATE Caracteristicas SET Icone = 'vista-mar' WHERE Icone IS NULL AND Nome = N'Vista mar';
                UPDATE Caracteristicas SET Icone = 'kitchenette' WHERE Icone IS NULL AND Nome = N'Kitchenette';
                UPDATE Caracteristicas SET Icone = 'banheira' WHERE Icone IS NULL AND Nome = N'Banheira';
                UPDATE RegimesAlimentares SET Icone = 'sem-refeicoes' WHERE Icone IS NULL AND Nome = N'Sem refeições';
                UPDATE RegimesAlimentares SET Icone = 'pequeno-almoco' WHERE Icone IS NULL AND Nome = N'Pequeno-almoço';
                UPDATE RegimesAlimentares SET Icone = 'meia-pensao' WHERE Icone IS NULL AND Nome = N'Meia pensão';
                UPDATE RegimesAlimentares SET Icone = 'pensao-completa' WHERE Icone IS NULL AND Nome = N'Pensão completa';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icone",
                table: "RegimesAlimentares");

            migrationBuilder.DropColumn(
                name: "Icone",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Icone",
                table: "Caracteristicas");
        }
    }
}
