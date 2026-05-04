using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SagradaFamilia.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NormalizacionCompleta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RolPermisos_Rol_OpcionAccionId",
                table: "RolPermisos");

            migrationBuilder.DropIndex(
                name: "IX_OpcionAcciones_OpcionId_Accion",
                table: "OpcionAcciones");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Rol",
                table: "RolPermisos");

            migrationBuilder.DropColumn(
                name: "Accion",
                table: "OpcionAcciones");

            migrationBuilder.AddColumn<int>(
                name: "RolId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RolId",
                table: "RolPermisos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccionId",
                table: "OpcionAcciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Acciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_RolId_OpcionAccionId",
                table: "RolPermisos",
                columns: new[] { "RolId", "OpcionAccionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpcionAcciones_AccionId",
                table: "OpcionAcciones",
                column: "AccionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpcionAcciones_OpcionId_AccionId",
                table: "OpcionAcciones",
                columns: new[] { "OpcionId", "AccionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Acciones_Nombre",
                table: "Acciones",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OpcionAcciones_Acciones_AccionId",
                table: "OpcionAcciones",
                column: "AccionId",
                principalTable: "Acciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RolPermisos_Roles_RolId",
                table: "RolPermisos",
                column: "RolId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Roles_RolId",
                table: "Usuarios",
                column: "RolId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OpcionAcciones_Acciones_AccionId",
                table: "OpcionAcciones");

            migrationBuilder.DropForeignKey(
                name: "FK_RolPermisos_Roles_RolId",
                table: "RolPermisos");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Roles_RolId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Acciones");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_RolPermisos_RolId_OpcionAccionId",
                table: "RolPermisos");

            migrationBuilder.DropIndex(
                name: "IX_OpcionAcciones_AccionId",
                table: "OpcionAcciones");

            migrationBuilder.DropIndex(
                name: "IX_OpcionAcciones_OpcionId_AccionId",
                table: "OpcionAcciones");

            migrationBuilder.DropColumn(
                name: "RolId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RolId",
                table: "RolPermisos");

            migrationBuilder.DropColumn(
                name: "AccionId",
                table: "OpcionAcciones");

            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "Usuarios",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Rol",
                table: "RolPermisos",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Accion",
                table: "OpcionAcciones",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_RolPermisos_Rol_OpcionAccionId",
                table: "RolPermisos",
                columns: new[] { "Rol", "OpcionAccionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpcionAcciones_OpcionId_Accion",
                table: "OpcionAcciones",
                columns: new[] { "OpcionId", "Accion" },
                unique: true);
        }
    }
}
