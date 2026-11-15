using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoHoteis.Migrations
{
    /// <inheritdoc />
    public partial class ValidacoesERegras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Alojamentos_HotelId",
                table: "Alojamentos");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Reservas_Datas",
                table: "Reservas",
                sql: "[DataSaida] > [DataEntrada]");

            migrationBuilder.CreateIndex(
                name: "IX_RegimesAlimentares_Nome",
                table: "RegimesAlimentares",
                column: "Nome",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_RegimesAlimentares_Percentagem",
                table: "RegimesAlimentares",
                sql: "[Percentagem] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_ModosDisponibilizacao_Nome",
                table: "ModosDisponibilizacao",
                column: "Nome",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_LinhasReserva_NumHospedes",
                table: "LinhasReserva",
                sql: "[NumHospedes] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LinhasReserva_Quantidade",
                table: "LinhasReserva",
                sql: "[Quantidade] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Hoteis_Nome",
                table: "Hoteis",
                column: "Nome",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Hoteis_Estrelas",
                table: "Hoteis",
                sql: "[Estrelas] BETWEEN 1 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Hoteis_Tipo",
                table: "Hoteis",
                sql: "[Tipo] IN (N'Hotel', N'Aparthotel')");

            migrationBuilder.CreateIndex(
                name: "IX_Faturas_Numero",
                table: "Faturas",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nome",
                table: "Categorias",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Caracteristicas_Nome",
                table: "Caracteristicas",
                column: "Nome",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Campanhas_Datas",
                table: "Campanhas",
                sql: "[DataFim] >= [DataInicio]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Campanhas_Percentagem",
                table: "Campanhas",
                sql: "[Percentagem] > -100 AND [Percentagem] <> 0");

            migrationBuilder.CreateIndex(
                name: "IX_Alojamentos_HotelId_Nome",
                table: "Alojamentos",
                columns: new[] { "HotelId", "Nome" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Alojamentos_Capacidade",
                table: "Alojamentos",
                sql: "[Capacidade] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Alojamentos_NumUnidades",
                table: "Alojamentos",
                sql: "[NumUnidades] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Alojamentos_PrecoNoite",
                table: "Alojamentos",
                sql: "[PrecoNoite] > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Reservas_Datas",
                table: "Reservas");

            migrationBuilder.DropIndex(
                name: "IX_RegimesAlimentares_Nome",
                table: "RegimesAlimentares");

            migrationBuilder.DropCheckConstraint(
                name: "CK_RegimesAlimentares_Percentagem",
                table: "RegimesAlimentares");

            migrationBuilder.DropIndex(
                name: "IX_ModosDisponibilizacao_Nome",
                table: "ModosDisponibilizacao");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LinhasReserva_NumHospedes",
                table: "LinhasReserva");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LinhasReserva_Quantidade",
                table: "LinhasReserva");

            migrationBuilder.DropIndex(
                name: "IX_Hoteis_Nome",
                table: "Hoteis");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Hoteis_Estrelas",
                table: "Hoteis");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Hoteis_Tipo",
                table: "Hoteis");

            migrationBuilder.DropIndex(
                name: "IX_Faturas_Numero",
                table: "Faturas");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_Nome",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Caracteristicas_Nome",
                table: "Caracteristicas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Campanhas_Datas",
                table: "Campanhas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Campanhas_Percentagem",
                table: "Campanhas");

            migrationBuilder.DropIndex(
                name: "IX_Alojamentos_HotelId_Nome",
                table: "Alojamentos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Alojamentos_Capacidade",
                table: "Alojamentos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Alojamentos_NumUnidades",
                table: "Alojamentos");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Alojamentos_PrecoNoite",
                table: "Alojamentos");

            migrationBuilder.CreateIndex(
                name: "IX_Alojamentos_HotelId",
                table: "Alojamentos",
                column: "HotelId");
        }
    }
}
