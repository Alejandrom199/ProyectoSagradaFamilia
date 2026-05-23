using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class MenuSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Modulos.AnyAsync())
                return;

            var accionesDb = await context.Acciones
                .ToDictionaryAsync(a => a.Nombre, a => a.Id, StringComparer.OrdinalIgnoreCase);

            int GetIdAccion(string nombre)
            {
                if (accionesDb.TryGetValue(nombre, out var id)) return id;
                throw new Exception($"ERROR: La acción '{nombre}' no existe en la tabla Acciones.");
            }

            var modulos = new List<Modulo>
            {
                new()
                {
                    Nombre = "Administración", Icono = "cog-6-tooth", Orden = 1, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Usuarios", Ruta = "/usuarios", Icono = "users", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                        new() { Nombre = "Padres de Familia", Ruta = "/padres", Icono = "user-group", Orden = 2, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                        new() { Nombre = "Médicos", Ruta = "/medicos", Icono = "identification", Orden = 3, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                        new() { Nombre = "Roles y Permisos", Ruta = "/roles", Icono = "shield-check", Orden = 4, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) },
                        new() { Nombre = "Parámetros del Sistema", Ruta = "/parametros", Icono = "cog-6-tooth", Orden = 5, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) }
                    }
                },
                new()
                {
                    Nombre = "Gestión de Pacientes", Icono = "heart", Orden = 2, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Pacientes", Ruta = "/pacientes", Icono = "heart", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                    }
                },
                new()
                {
                    Nombre = "Atención Médica", Icono = "clipboard-document-list", Orden = 3, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Agenda de Citas", Ruta = "/citas", Icono = "calendar-days", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                        new() { Nombre = "Historial de Citas", Ruta = "/citas/historial", Icono = "clock", Orden = 2, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) },
                        new() { Nombre = "Historial de Prescripciones", Ruta = "/prescripciones", Icono = "clipboard-document-list", Orden = 3, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) }
                    }
                },
                new()
                {
                    Nombre = "Predicciones", Icono = "arrow-trending-up", Orden = 4, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Predicciones de Crecimiento", Ruta = "/predicciones", Icono = "arrow-trending-up", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) }
                    }
                },
                new()
                {
                    Nombre = "Orientación Alimentaria", Icono = "archive-box", Orden = 5, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Alimentos", Ruta = "/alimentos", Icono = "archive-box", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) }
                    }
                },
                new()
                {
                    Nombre = "Monitoreo", Icono = "table-cells", Orden = 6, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Actividad del Sistema", Ruta = "/sistema/auditoria", Icono = "magnifying-glass", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) },
                        new() { Nombre = "Eventos del Sistema", Ruta = "/sistema/logs", Icono = "exclamation-triangle", Orden = 2, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) }
                    }
                },
            };

            context.Modulos.AddRange(modulos);
            await context.SaveChangesAsync();

            await SeedRolPermisosAsync(context, GetIdAccion("Ver"));
        }

        private static async Task SeedRolPermisosAsync(AppDbContext context, int idAccionVer)
        {
            int idAdmin = (int)RolEnum.Administrador;
            int idMedico = (int)RolEnum.Medico;
            int idPadre = (int)RolEnum.Padre;

            var todasLasOpcionAcciones = await context.OpcionAcciones
                .Include(oa => oa.Opcion)
                    .ThenInclude(o => o.Modulo)
                .Include(oa => oa.Accion)
                .ToListAsync();

            var rolPermisos = new List<RolPermiso>();

            foreach (var oa in todasLasOpcionAcciones)
            {
                rolPermisos.Add(new RolPermiso { RolId = idAdmin, OpcionAccionId = oa.Id, Permitido = true });

                var modulo = oa.Opcion.Modulo.Nombre;
                var opcion = oa.Opcion.Nombre;
                var accion = oa.Accion.Nombre;


                bool medicoTieneAcceso =
                    (modulo == "Administración" && opcion == "Padres de Familia") ||
                    modulo == "Gestión de Pacientes" ||
                    modulo == "Atención Médica" ||
                    modulo == "Predicciones" ||
                    modulo == "Orientación Alimentaria" ||
                    (modulo == "Monitoreo" && opcion == "Actividad del Sistema");

                if (medicoTieneAcceso)
                    rolPermisos.Add(new RolPermiso { RolId = idMedico, OpcionAccionId = oa.Id, Permitido = true });

                if (modulo == "Gestión de Pacientes" && accion == "Ver")
                    rolPermisos.Add(new RolPermiso { RolId = idPadre, OpcionAccionId = oa.Id, Permitido = true });
            }

            context.RolPermisos.AddRange(rolPermisos);
            await context.SaveChangesAsync();
        }

        private static List<OpcionAccion> Acciones(params int[] accionIds) =>
            accionIds.Select(id => new OpcionAccion { AccionId = id }).ToList();
    }
}