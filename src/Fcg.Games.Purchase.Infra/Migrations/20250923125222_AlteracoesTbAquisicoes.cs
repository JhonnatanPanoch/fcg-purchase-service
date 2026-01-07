using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fcg.Games.Purchase.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AlteracoesTbAquisicoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aquisicoes");

            migrationBuilder.CreateTable(
                name: "TransacoesJogos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompraId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    JogoId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataAquisicao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PrecoPago = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransacoesJogos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransacoesJogos");

            migrationBuilder.CreateTable(
                name: "Aquisicoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DataAquisicao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    JogoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PrecoPago = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aquisicoes", x => x.Id);
                });
        }
    }
}
