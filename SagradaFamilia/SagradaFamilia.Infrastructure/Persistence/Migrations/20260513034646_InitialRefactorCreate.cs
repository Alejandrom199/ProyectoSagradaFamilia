using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SagradaFamilia.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialRefactorCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OMS_PesoPorEdad");

            migrationBuilder.DropTable(
                name: "OMS_TallaPorEdad");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Opciones");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Modulos");

            migrationBuilder.DropColumn(
                name: "NumeroColegiatura",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "LogsSistema");

            migrationBuilder.RenameColumn(
                name: "PesoReal",
                table: "Predicciones",
                newName: "ValorReal");

            migrationBuilder.RenameColumn(
                name: "PesoPredicho",
                table: "Predicciones",
                newName: "ValorPredicho");

            migrationBuilder.RenameColumn(
                name: "PesoMinimo",
                table: "Predicciones",
                newName: "ValorMinimo");

            migrationBuilder.RenameColumn(
                name: "PesoMaximo",
                table: "Predicciones",
                newName: "ValorMaximo");

            migrationBuilder.RenameColumn(
                name: "Meses",
                table: "Predicciones",
                newName: "Tipo");

            migrationBuilder.RenameColumn(
                name: "EdadMinimaIntro",
                table: "Alimentos",
                newName: "UsuarioCreacionId");

            migrationBuilder.RenameColumn(
                name: "EdadMaxima",
                table: "Alimentos",
                newName: "UsuarioModificacionId");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioModificacionId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProyeccionMeses",
                table: "Predicciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MedicoId",
                table: "Padres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "Padres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "Padres",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioModificacionId",
                table: "Padres",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "Opciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "Opciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioModificacionId",
                table: "Opciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicoId",
                table: "Ninos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "Ninos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "Ninos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioModificacionId",
                table: "Ninos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "Modulos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "Modulos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioModificacionId",
                table: "Modulos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "Medidas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "Medidas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioModificacionId",
                table: "Medidas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "Medicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "Medicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioModificacionId",
                table: "Medicos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioCreacionId",
                table: "CategoriasAlimentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "CategoriasAlimentos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioModificacionId",
                table: "CategoriasAlimentos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadMinimaMeses",
                table: "Alimentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioEliminacionId",
                table: "Alimentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tabla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClavePrimaria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ValoresAntiguos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValoresNuevos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auditorias_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NinoId = table.Column<int>(type: "int", nullable: false),
                    MedicoId = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Citas_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citas_Ninos_NinoId",
                        column: x => x.NinoId,
                        principalTable: "Ninos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OMS_Referencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sexo = table.Column<string>(type: "char(1)", nullable: false),
                    EdadMeses = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Percentil3 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil15 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil50 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil85 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil97 = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OMS_Referencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prescripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NinoId = table.Column<int>(type: "int", nullable: false),
                    MedicoId = table.Column<int>(type: "int", nullable: false),
                    DetalleMedicamentos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Indicaciones = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificacionId = table.Column<int>(type: "int", nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioEliminacionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescripciones_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prescripciones_Ninos_NinoId",
                        column: x => x.NinoId,
                        principalTable: "Ninos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Eliminado",
                table: "Usuarios",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Padres_Eliminado",
                table: "Padres",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Padres_MedicoId",
                table: "Padres",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Opciones_Eliminado",
                table: "Opciones",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Ninos_Eliminado",
                table: "Ninos",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Ninos_MedicoId",
                table: "Ninos",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_Eliminado",
                table: "Modulos",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Medidas_Eliminado",
                table: "Medidas",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Medicos_Eliminado",
                table: "Medicos",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasAlimentos_Eliminado",
                table: "CategoriasAlimentos",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Alimentos_Eliminado",
                table: "Alimentos",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_Fecha",
                table: "Auditorias",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_Tabla",
                table: "Auditorias",
                column: "Tabla");

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_UsuarioId",
                table: "Auditorias",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_Eliminado",
                table: "Citas",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_MedicoId_FechaHora",
                table: "Citas",
                columns: new[] { "MedicoId", "FechaHora" },
                unique: true,
                filter: "[Estado] != 2 AND [Eliminado] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Citas_NinoId",
                table: "Citas",
                column: "NinoId");

            migrationBuilder.CreateIndex(
                name: "IX_OMS_Referencias_Sexo_EdadMeses_Tipo",
                table: "OMS_Referencias",
                columns: new[] { "Sexo", "EdadMeses", "Tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescripciones_Eliminado",
                table: "Prescripciones",
                column: "Eliminado");

            migrationBuilder.CreateIndex(
                name: "IX_Prescripciones_MedicoId",
                table: "Prescripciones",
                column: "MedicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescripciones_NinoId",
                table: "Prescripciones",
                column: "NinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ninos_Medicos_MedicoId",
                table: "Ninos",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Padres_Medicos_MedicoId",
                table: "Padres",
                column: "MedicoId",
                principalTable: "Medicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ninos_Medicos_MedicoId",
                table: "Ninos");

            migrationBuilder.DropForeignKey(
                name: "FK_Padres_Medicos_MedicoId",
                table: "Padres");

            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "OMS_Referencias");

            migrationBuilder.DropTable(
                name: "Prescripciones");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Eliminado",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Padres_Eliminado",
                table: "Padres");

            migrationBuilder.DropIndex(
                name: "IX_Padres_MedicoId",
                table: "Padres");

            migrationBuilder.DropIndex(
                name: "IX_Opciones_Eliminado",
                table: "Opciones");

            migrationBuilder.DropIndex(
                name: "IX_Ninos_Eliminado",
                table: "Ninos");

            migrationBuilder.DropIndex(
                name: "IX_Ninos_MedicoId",
                table: "Ninos");

            migrationBuilder.DropIndex(
                name: "IX_Modulos_Eliminado",
                table: "Modulos");

            migrationBuilder.DropIndex(
                name: "IX_Medidas_Eliminado",
                table: "Medidas");

            migrationBuilder.DropIndex(
                name: "IX_Medicos_Eliminado",
                table: "Medicos");

            migrationBuilder.DropIndex(
                name: "IX_CategoriasAlimentos_Eliminado",
                table: "CategoriasAlimentos");

            migrationBuilder.DropIndex(
                name: "IX_Alimentos_Eliminado",
                table: "Alimentos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "ProyeccionMeses",
                table: "Predicciones");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "Padres");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Padres");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "Padres");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Padres");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Opciones");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "Opciones");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Opciones");

            migrationBuilder.DropColumn(
                name: "MedicoId",
                table: "Ninos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Ninos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "Ninos");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Ninos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Modulos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "Modulos");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Modulos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Medidas");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "Medidas");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Medidas");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "Medicos");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "CategoriasAlimentos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "CategoriasAlimentos");

            migrationBuilder.DropColumn(
                name: "UsuarioModificacionId",
                table: "CategoriasAlimentos");

            migrationBuilder.DropColumn(
                name: "EdadMinimaMeses",
                table: "Alimentos");

            migrationBuilder.DropColumn(
                name: "UsuarioEliminacionId",
                table: "Alimentos");

            migrationBuilder.RenameColumn(
                name: "ValorReal",
                table: "Predicciones",
                newName: "PesoReal");

            migrationBuilder.RenameColumn(
                name: "ValorPredicho",
                table: "Predicciones",
                newName: "PesoPredicho");

            migrationBuilder.RenameColumn(
                name: "ValorMinimo",
                table: "Predicciones",
                newName: "PesoMinimo");

            migrationBuilder.RenameColumn(
                name: "ValorMaximo",
                table: "Predicciones",
                newName: "PesoMaximo");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Predicciones",
                newName: "Meses");

            migrationBuilder.RenameColumn(
                name: "UsuarioModificacionId",
                table: "Alimentos",
                newName: "EdadMaxima");

            migrationBuilder.RenameColumn(
                name: "UsuarioCreacionId",
                table: "Alimentos",
                newName: "EdadMinimaIntro");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Opciones",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Modulos",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroColegiatura",
                table: "Medicos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "LogsSistema",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OMS_PesoPorEdad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EdadMeses = table.Column<int>(type: "int", nullable: false),
                    Percentil15 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil3 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil50 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil85 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil97 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Sexo = table.Column<string>(type: "char(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OMS_PesoPorEdad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OMS_TallaPorEdad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EdadMeses = table.Column<int>(type: "int", nullable: false),
                    Percentil15 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil3 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil50 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil85 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Percentil97 = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Sexo = table.Column<string>(type: "char(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OMS_TallaPorEdad", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OMS_PesoPorEdad_Sexo_EdadMeses",
                table: "OMS_PesoPorEdad",
                columns: new[] { "Sexo", "EdadMeses" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OMS_TallaPorEdad_Sexo_EdadMeses",
                table: "OMS_TallaPorEdad",
                columns: new[] { "Sexo", "EdadMeses" },
                unique: true);
        }
    }
}
