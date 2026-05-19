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
                    Nombre = "Gestión de Usuarios", Icono = "users", Orden = 1, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Padres de Familia", Ruta = "/padres", Icono = "user-group", Orden = 2, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) }
                    }
                },
                new()
                {
                    Nombre = "Gestión de Pacientes", Icono = "baby", Orden = 2, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Pacientes", Ruta = "/pacientes", Icono = "child", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                    }
                },
                new()
                {
                    Nombre = "Atención Médica", Icono = "stethoscope", Orden = 3, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Agenda de Citas", Ruta = "/citas", Icono = "calendar-days", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) },
                        new() { Nombre = "Historial de Citas", Ruta = "/citas/historial", Icono = "clock", Orden = 2, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) },
                        new() { Nombre = "Historial de Prescripciones", Ruta = "/prescripciones", Icono = "pills", Orden = 3, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) }
                    }
                },
                new()
                {
                    Nombre = "Predicciones", Icono = "chart-line", Orden = 3, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Predicciones de Crecimiento", Ruta = "/predicciones", Icono = "trending-up", Orden = 1, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver")) }
                    }
                },
                new()
                {
                    Nombre = "Orientación Alimentaria", Icono = "apple", Orden = 4, Activo = true,
                    Opciones = new List<Opcion>
                    {
                        new() { Nombre = "Alimentos", Ruta = "/alimentos", Icono = "food", Orden = 2, Activo = true,
                            OpcionAcciones = Acciones(GetIdAccion("Ver"), GetIdAccion("Crear"), GetIdAccion("Editar"), GetIdAccion("Eliminar")) }
                    }
                }
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
                .ToListAsync();

            var rolPermisos = new List<RolPermiso>();

            foreach (var oa in todasLasOpcionAcciones)
            {
                rolPermisos.Add(new RolPermiso { RolId = idAdmin, OpcionAccionId = oa.Id, Permitido = true });

                rolPermisos.Add(new RolPermiso { RolId = idMedico, OpcionAccionId = oa.Id, Permitido = true });

                bool esModuloUsuarios = oa.Opcion.Modulo.Nombre == "Gestión de Usuarios";
                bool esAccionVer = oa.AccionId == idAccionVer;

                if (!esModuloUsuarios && esAccionVer)
                {
                    rolPermisos.Add(new RolPermiso { RolId = idPadre, OpcionAccionId = oa.Id, Permitido = true });
                }
            }

            context.RolPermisos.AddRange(rolPermisos);
            await context.SaveChangesAsync();
        }

        private static List<OpcionAccion> Acciones(params int[] accionIds) =>
            accionIds.Select(id => new OpcionAccion { AccionId = id }).ToList();
    }
}