using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SagradaFamilia.Infrastructure.Persistence.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class SyncNinoMedicoConPadre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Corrige datos: antes de este fix, reasignar el médico de un padre (o el
            // padre de un niño) no propagaba el cambio a Nino.MedicoId, dejando niños
            // apuntando al médico anterior en vez del médico de su padre actual.
            migrationBuilder.Sql(@"
                UPDATE ""Ninos"" n
                SET ""MedicoId"" = p.""MedicoId""
                FROM ""Padres"" p
                WHERE n.""PadreId"" = p.""Id""
                  AND n.""MedicoId"" <> p.""MedicoId""
                  AND n.""Eliminado"" = false;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
