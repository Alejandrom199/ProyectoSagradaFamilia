using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SagradaFamilia.Infrastructure.Persistence.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class AddCitaOrigenIdYEstadoReagendada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Citas_MedicoId_FechaHora",
                table: "Citas");

            migrationBuilder.AddColumn<int>(
                name: "CitaOrigenId",
                table: "Citas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Citas_CitaOrigenId",
                table: "Citas",
                column: "CitaOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoId_FechaHora",
                table: "Citas",
                columns: new[] { "MedicoId", "FechaHora" },
                unique: true,
                filter: "\"Estado\" != 5 AND \"Estado\" != 6 AND \"Eliminado\" = false");

            migrationBuilder.AddForeignKey(
                name: "FK_Citas_Citas_CitaOrigenId",
                table: "Citas",
                column: "CitaOrigenId",
                principalTable: "Citas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Citas_Citas_CitaOrigenId",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_CitaOrigenId",
                table: "Citas");

            migrationBuilder.DropIndex(
                name: "IX_Citas_MedicoId_FechaHora",
                table: "Citas");

            migrationBuilder.DropColumn(
                name: "CitaOrigenId",
                table: "Citas");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoId_FechaHora",
                table: "Citas",
                columns: new[] { "MedicoId", "FechaHora" },
                unique: true,
                filter: "\"Estado\" != 5 AND \"Eliminado\" = false");
        }
    }
}
