using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SagradaFamilia.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TrazabilidadCitaPrescripcion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CitaId",
                table: "Prescripciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Diagnostico",
                table: "Prescripciones",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NinoId1",
                table: "Prescripciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotasConsulta",
                table: "Citas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescripciones_CitaId",
                table: "Prescripciones",
                column: "CitaId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescripciones_Citas_CitaId",
                table: "Prescripciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescripciones_Ninos_NinoId1",
                table: "Prescripciones");

            migrationBuilder.DropIndex(
                name: "IX_Prescripciones_CitaId",
                table: "Prescripciones");

            migrationBuilder.DropIndex(
                name: "IX_Prescripciones_NinoId1",
                table: "Prescripciones");

            migrationBuilder.DropColumn(
                name: "CitaId",
                table: "Prescripciones");

            migrationBuilder.DropColumn(
                name: "Diagnostico",
                table: "Prescripciones");

            migrationBuilder.DropColumn(
                name: "NinoId1",
                table: "Prescripciones");

            migrationBuilder.DropColumn(
                name: "NotasConsulta",
                table: "Citas");
        }
    }
}
