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
                    Nombre = "Administración", Icono = "settings", Orden = 1, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Usuarios", Ruta = "/usuarios", Icono = "people", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                        new() { Nombre = "Padres de Familia", Ruta = "/padres", Icono = "groups", Orden = 2, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                        new() { Nombre = "Médicos", Ruta = "/medicos", Icono = "user-doctor", Orden = 3, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                        new() { Nombre = "Roles y Permisos", Ruta = "/roles", Icono = "verified-user", Orden = 4, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) },
                        new() { Nombre = "Parámetros del Sistema", Ruta = "/parametros", Icono = "tune", Orden = 5, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                        new() { Nombre = "Catálogos", Ruta = "/catalogos", Icono = "category", Orden = 6, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) }
                    }
                },
                new()
                {
                    Nombre = "Gestión de Pacientes", Icono = "monitor-heart", Orden = 2, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Pacientes", Ruta = "/pacientes", Icono = "child-care", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                    }
                },
                new()
                {
                    Nombre = "Atención Médica", Icono = "notes-medical", Orden = 3, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Agenda de Citas", Ruta = "/citas", Icono = "calendar-check", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                        new() { Nombre = "Historial de Citas", Ruta = "/citas/historial", Icono = "history", Orden = 2, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) },
                        new() { Nombre = "Historial de Prescripciones", Ruta = "/prescripciones", Icono = "file-prescription", Orden = 3, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) }
                    }
                },
                new()
                {
                    Nombre = "Predicciones", Icono = "brain", Orden = 4, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Predicciones de Crecimiento", Ruta = "/predicciones", Icono = "chart-line", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) }
                    }
                },
                new()
                {
                    Nombre = "Orientación Alimentaria", Icono = "set-meal", Orden = 5, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Alimentos", Ruta = "/alimentos", Icono = "utensils", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) }
                    }
                },
                new()
                {
                    Nombre = "Monitoreo", Icono = "table-chart", Orden = 6, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Actividad del Sistema", Ruta = "/sistema/auditoria", Icono = "search", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) },
                        new() { Nombre = "Eventos del Sistema", Ruta = "/sistema/logs", Icono = "warning-amber", Orden = 2, Activo = true,
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
                var modulo = oa.Opcion.Modulo.Nombre;
                var opcion = oa.Opcion.Nombre;
                var accion = oa.Accion.Nombre;

                // Administrador: solo secciones de gestión y monitoreo (sin clínica)
                bool adminTieneAcceso =
                    modulo == "Administración" ||
                    modulo == "Monitoreo";

                if (adminTieneAcceso)
                    rolPermisos.Add(new RolPermiso { RolId = idAdmin, OpcionAccionId = oa.Id, Permitido = true });

                // Médico: secciones clínicas + padres de familia + monitoreo básico
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