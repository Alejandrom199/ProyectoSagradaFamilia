using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SagradaFamilia.Infrastructure.Persistence.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class AddCitaFechaHoraFin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaHoraFin",
                table: "Citas",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaHoraFin",
                table: "Citas");
        }
    }
}
