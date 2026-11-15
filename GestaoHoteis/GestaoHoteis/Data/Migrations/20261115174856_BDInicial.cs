using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoHoteis.Migrations
{
    /// <inheritdoc />
    public partial class BDInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apelido",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataRegisto",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "Fotografia",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatriculaVeiculo",
                table: "AspNetUsers",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MoradaId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "NIF",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telemovel",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Caracteristicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caracteristicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModosDisponibilizacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModosDisponibilizacao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moradas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rua = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoPostal = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Localidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Distrito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moradas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegimesAlimentares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Percentagem = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegimesAlimentares", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hoteis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estrelas = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UrlImagem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Imagem = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    MoradaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hoteis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hoteis_Moradas_MoradaId",
                        column: x => x.MoradaId,
                        principalTable: "Moradas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alojamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Capacidade = table.Column<int>(type: "int", nullable: false),
                    NumUnidades = table.Column<int>(type: "int", nullable: false),
                    PrecoNoite = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UrlImagem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Imagem = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    ModoDisponibilizacaoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alojamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alojamentos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alojamentos_Hoteis_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hoteis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alojamentos_ModosDisponibilizacao_ModoDisponibilizacaoId",
                        column: x => x.ModoDisponibilizacaoId,
                        principalTable: "ModosDisponibilizacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Campanhas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Percentagem = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campanhas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campanhas_Hoteis_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hoteis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    MoradaFaturacaoId = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataSaida = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservas_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Hoteis_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hoteis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservas_Moradas_MoradaFaturacaoId",
                        column: x => x.MoradaFaturacaoId,
                        principalTable: "Moradas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlojamentoCaracteristica",
                columns: table => new
                {
                    alojamentosId = table.Column<int>(type: "int", nullable: false),
                    caracteristicasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlojamentoCaracteristica", x => new { x.alojamentosId, x.caracteristicasId });
                    table.ForeignKey(
                        name: "FK_AlojamentoCaracteristica_Alojamentos_alojamentosId",
                        column: x => x.alojamentosId,
                        principalTable: "Alojamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlojamentoCaracteristica_Caracteristicas_caracteristicasId",
                        column: x => x.caracteristicasId,
                        principalTable: "Caracteristicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlojamentoRegimeAlimentar",
                columns: table => new
                {
                    alojamentosId = table.Column<int>(type: "int", nullable: false),
                    regimesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlojamentoRegimeAlimentar", x => new { x.alojamentosId, x.regimesId });
                    table.ForeignKey(
                        name: "FK_AlojamentoRegimeAlimentar_Alojamentos_alojamentosId",
                        column: x => x.alojamentosId,
                        principalTable: "Alojamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlojamentoRegimeAlimentar_RegimesAlimentares_regimesId",
                        column: x => x.regimesId,
                        principalTable: "RegimesAlimentares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Faturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservaId = table.Column<int>(type: "int", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NomeCliente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NIFCliente = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faturas_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LinhasReserva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservaId = table.Column<int>(type: "int", nullable: false),
                    AlojamentoId = table.Column<int>(type: "int", nullable: false),
                    RegimeAlimentarId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    NumHospedes = table.Column<int>(type: "int", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinhasReserva", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LinhasReserva_Alojamentos_AlojamentoId",
                        column: x => x.AlojamentoId,
                        principalTable: "Alojamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LinhasReserva_RegimesAlimentares_RegimeAlimentarId",
                        column: x => x.RegimeAlimentarId,
                        principalTable: "RegimesAlimentares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LinhasReserva_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservaId = table.Column<int>(type: "int", nullable: false),
                    Metodo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Reservas_ReservaId",
                        column: x => x.ReservaId,
                        principalTable: "Reservas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MoradaId",
                table: "AspNetUsers",
                column: "MoradaId",
                unique: true,
                filter: "[MoradaId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AlojamentoCaracteristica_caracteristicasId",
                table: "AlojamentoCaracteristica",
                column: "caracteristicasId");

            migrationBuilder.CreateIndex(
                name: "IX_AlojamentoRegimeAlimentar_regimesId",
                table: "AlojamentoRegimeAlimentar",
                column: "regimesId");

            migrationBuilder.CreateIndex(
                name: "IX_Alojamentos_CategoriaId",
                table: "Alojamentos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Alojamentos_HotelId",
                table: "Alojamentos",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Alojamentos_ModoDisponibilizacaoId",
                table: "Alojamentos",
                column: "ModoDisponibilizacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Campanhas_HotelId",
                table: "Campanhas",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturas_ReservaId",
                table: "Faturas",
                column: "ReservaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hoteis_MoradaId",
                table: "Hoteis",
                column: "MoradaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LinhasReserva_AlojamentoId",
                table: "LinhasReserva",
                column: "AlojamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_LinhasReserva_RegimeAlimentarId",
                table: "LinhasReserva",
                column: "RegimeAlimentarId");

            migrationBuilder.CreateIndex(
                name: "IX_LinhasReserva_ReservaId",
                table: "LinhasReserva",
                column: "ReservaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_ReservaId",
                table: "Pagamentos",
                column: "ReservaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_ClienteId",
                table: "Reservas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_HotelId",
                table: "Reservas",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservas_MoradaFaturacaoId",
                table: "Reservas",
                column: "MoradaFaturacaoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Moradas_MoradaId",
                table: "AspNetUsers",
                column: "MoradaId",
                principalTable: "Moradas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Moradas_MoradaId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AlojamentoCaracteristica");

            migrationBuilder.DropTable(
                name: "AlojamentoRegimeAlimentar");

            migrationBuilder.DropTable(
                name: "Campanhas");

            migrationBuilder.DropTable(
                name: "Faturas");

            migrationBuilder.DropTable(
                name: "LinhasReserva");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "Caracteristicas");

            migrationBuilder.DropTable(
                name: "Alojamentos");

            migrationBuilder.DropTable(
                name: "RegimesAlimentares");

            migrationBuilder.DropTable(
                name: "Reservas");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "ModosDisponibilizacao");

            migrationBuilder.DropTable(
                name: "Hoteis");

            migrationBuilder.DropTable(
                name: "Moradas");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MoradaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Apelido",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DataRegisto",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Fotografia",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MatriculaVeiculo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MoradaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NIF",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Telemovel",
                table: "AspNetUsers");
        }
    }
}
