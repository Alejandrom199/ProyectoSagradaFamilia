using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SagradaFamilia.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorFinalNutriBaby : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medidas_Usuarios_MedicoId",
                table: "Medidas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ninos_Usuarios_RepresentanteId",
                table: "Ninos");

            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "RepresentanteId",
                table: "Ninos",
                newName: "PadreId");

            migrationBuilder.RenameIndex(
                name: "IX_Ninos_RepresentanteId",
                table: "Ninos",
                newName: "IX_Ninos_PadreId");

            migrationBuilder.CreateTable(
                name: "Medicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Especialidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NumeroColegiatura = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Padres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Padres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Padres_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_UsuarioId",
                table: "Medicos",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Padres_UsuarioId",
                table: "Padres",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Medidas_Medicos_MedicoId",
                table: "Medidas",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ninos_Padres_PadreId",
                table: "Ninos",
                column: "PadreId",
                principalTable: "Padres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medidas_Medicos_MedicoId",
                table: "Medidas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ninos_Padres_PadreId",
                table: "Ninos");

            migrationBuilder.DropTable(
                name: "Medicos");

            migrationBuilder.DropTable(
                name: "Padres");

            migrationBuilder.RenameColumn(
                name: "PadreId",
                table: "Ninos",
                newName: "RepresentanteId");

            migrationBuilder.RenameIndex(
                name: "IX_Ninos_PadreId",
                table: "Ninos",
                newName: "IX_Ninos_RepresentanteId");

            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Usuarios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Usuarios",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Usuarios",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Medidas_Usuarios_MedicoId",
                table: "Medidas",
                column: "MedicoId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ninos_Usuarios_RepresentanteId",
                table: "Ninos",
                column: "RepresentanteId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
