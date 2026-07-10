using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SagradaFamilia.Infrastructure.Persistence.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class AddReportesFirmaMedicoYHistorialEstadoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirmaActualizadaEn",
                table: "Medicos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirmaImagen",
                table: "Medicos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReportesGenerados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Uid = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    NinoId = table.Column<int>(type: "integer", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    UsuarioGeneradorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesGenerados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesGenerados_Ninos_NinoId",
                        column: x => x.NinoId,
                        principalTable: "Ninos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportesGenerados_Usuarios_UsuarioGeneradorId",
                        column: x => x.UsuarioGeneradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioEstadoHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    EstadoNuevo = table.Column<bool>(type: "boolean", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCambio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    UsuarioQueRealizoCambioId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioEstadoHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioEstadoHistorial_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioEstadoHistorial_Usuarios_UsuarioQueRealizoCambioId",
                        column: x => x.UsuarioQueRealizoCambioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportesGenerados_NinoId",
                table: "ReportesGenerados",
                column: "NinoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesGenerados_Tipo_FechaGeneracion",
                table: "ReportesGenerados",
                columns: new[] { "Tipo", "FechaGeneracion" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportesGenerados_Uid",
                table: "ReportesGenerados",
                column: "Uid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportesGenerados_UsuarioGeneradorId",
                table: "ReportesGenerados",
                column: "UsuarioGeneradorId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEstadoHistorial_UsuarioId",
                table: "UsuarioEstadoHistorial",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEstadoHistorial_UsuarioQueRealizoCambioId",
                table: "UsuarioEstadoHistorial",
                column: "UsuarioQueRealizoCambioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportesGenerados");

            migrationBuilder.DropTable(
                name: "UsuarioEstadoHistorial");

            migrationBuilder.DropColumn(
                name: "FirmaActualizadaEn",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "FirmaImagen",
                table: "Medicos");
        }
    }
}
