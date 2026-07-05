using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SagradaFamilia.Infrastructure.Persistence.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class AddConsultaYMedicamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescripciones_Ninos_NinoId1",
                table: "Prescripciones");

            migrationBuilder.DropIndex(
                name: "IX_Prescripciones_NinoId1",
                table: "Prescripciones");

            migrationBuilder.DropColumn(
                name: "NinoId1",
                table: "Prescripciones");

            migrationBuilder.CreateTable(
                name: "Consultas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CitaId = table.Column<int>(type: "integer", nullable: false),
                    NinoId = table.Column<int>(type: "integer", nullable: false),
                    MedicoId = table.Column<int>(type: "integer", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Diagnostico = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Indicaciones = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Evolucion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    UsuarioCreacionId = table.Column<int>(type: "integer", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "integer", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UsuarioEliminacionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consultas_Citas_CitaId",
                        column: x => x.CitaId,
                        principalTable: "Citas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_Ninos_NinoId",
                        column: x => x.NinoId,
                        principalTable: "Ninos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Medicamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PrescripcionId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Presentacion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Dosis = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Frecuencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ViaAdministracion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Duracion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cantidad = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    UsuarioCreacionId = table.Column<int>(type: "integer", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "integer", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UsuarioEliminacionId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicamentos_Prescripciones_PrescripcionId",
                        column: x => x.PrescripcionId,
                        principalTable: "Prescripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_CitaId",
                table: "Consultas",
                column: "CitaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_Eliminado",
                table: "Consultas",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_MedicoId",
                table: "Consultas",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_NinoId",
                table: "Consultas",
                column: "NinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_Eliminado",
                table: "Medicamentos",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_PrescripcionId",
                table: "Medicamentos",
                column: "PrescripcionId");

            // ── Backfill de datos (Cita → Consulta → Prescripcion/Medicamento) ──
            // En este punto: Consultas/Medicamentos existen vacías, Prescripciones.CitaId
            // todavía tiene los valores viejos (apunta a Citas.Id), y DetalleMedicamentos/
            // Diagnostico todavía existen. Todo esto se hace ANTES de renombrar la columna
            // y de dropear las columnas viejas, para no perder la referencia.

            migrationBuilder.Sql(@"
                INSERT INTO ""Consultas"" (""CitaId"", ""NinoId"", ""MedicoId"", ""Motivo"", ""Evolucion"", ""Estado"", ""FechaCreacion"", ""UsuarioCreacionId"", ""Eliminado"")
                SELECT c.""Id"", c.""NinoId"", c.""MedicoId"", c.""Motivo"", c.""NotasConsulta"",
                       CASE WHEN c.""Estado"" = 3 THEN 1 ELSE 2 END,
                       c.""FechaCreacion"", c.""UsuarioCreacionId"", false
                FROM ""Citas"" c
                WHERE c.""Estado"" IN (2, 3) AND c.""Eliminado"" = false;
            ");

            migrationBuilder.Sql(@"
                INSERT INTO ""Medicamentos"" (""PrescripcionId"", ""Nombre"", ""Observaciones"", ""Dosis"", ""Frecuencia"", ""ViaAdministracion"", ""FechaCreacion"", ""UsuarioCreacionId"", ""Eliminado"")
                SELECT p.""Id"", 'Ver observaciones', p.""DetalleMedicamentos"", 'No especificada', 'No especificada', 'No especificada', p.""FechaCreacion"", p.""UsuarioCreacionId"", false
                FROM ""Prescripciones"" p;
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Prescripciones"" p
                SET ""CitaId"" = co.""Id""
                FROM ""Consultas"" co
                WHERE co.""CitaId"" = p.""CitaId"";
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescripciones_Citas_CitaId",
                table: "Prescripciones");

            migrationBuilder.DropColumn(
                name: "DetalleMedicamentos",
                table: "Prescripciones");

            migrationBuilder.DropColumn(
                name: "Diagnostico",
                table: "Prescripciones");

            migrationBuilder.RenameColumn(
                name: "CitaId",
                table: "Prescripciones",
                newName: "ConsultaId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescripciones_CitaId",
                table: "Prescripciones",
                newName: "IX_Prescripciones_ConsultaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescripciones_Consultas_ConsultaId",
                table: "Prescripciones",
                column: "ConsultaId",
                principalTable: "Consultas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescripciones_Consultas_ConsultaId",
                table: "Prescripciones");

            migrationBuilder.DropTable(
                name: "Consultas");

            migrationBuilder.DropTable(
                name: "Medicamentos");

            migrationBuilder.RenameColumn(
                name: "ConsultaId",
                table: "Prescripciones",
                newName: "CitaId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescripciones_ConsultaId",
                table: "Prescripciones",
                newName: "IX_Prescripciones_CitaId");

            migrationBuilder.AddColumn<string>(
                name: "DetalleMedicamentos",
                table: "Prescripciones",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Diagnostico",
                table: "Prescripciones",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NinoId1",
                table: "Prescripciones",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescripciones_NinoId1",
                table: "Prescripciones",
                column: "NinoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescripciones_Citas_CitaId",
                table: "Prescripciones",
                column: "CitaId",
                principalTable: "Citas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescripciones_Ninos_NinoId1",
                table: "Prescripciones",
                column: "NinoId1",
                principalTable: "Ninos",
                principalColumn: "Id");
        }
    }
}
