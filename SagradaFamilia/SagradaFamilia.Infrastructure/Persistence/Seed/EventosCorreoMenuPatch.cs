using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Agrega la opción "Eventos de correo" al módulo Administración si aún no existe.
    /// Idempotente: puede ejecutarse múltiples veces sin efectos secundarios.
    /// </summary>
    public static class EventosCorreoMenuPatch
    {
        private const string Ruta = "/sistema/eventos-correo";

        public static async Task PatchAsync(AppDbContext context)
        {
            if (await context.Opciones.AnyAsync(o => o.Ruta == Ruta))
                return;

            var moduloAdmin = await context.Modulos
                .FirstOrDefaultAsync(m => m.Nombre == "Administración");

            if (moduloAdmin is null) return;

            var acciones = await context.Acciones
                .ToDictionaryAsync(a => a.Nombre, a => a.Id, StringComparer.OrdinalIgnoreCase);

            int IdAccion(string nombre) => acciones.TryGetValue(nombre, out var id)
                ? id
                : throw new Exception($"Acción '{nombre}' no encontrada.");

            var opcion = new Opcion
            {
                ModuloId = moduloAdmin.Id,
                Nombre   = "Eventos de correo",
                Ruta     = Ruta,
                Icono    = "mail",
                Orden    = 7,
                Activo   = true,
                OpcionAcciones = new List<OpcionAccion>
                {
                    new() { AccionId = IdAccion("Ver") },
                    new() { AccionId = IdAccion("Editar") },
                }
            };

            context.Opciones.Add(opcion);
            await context.SaveChangesAsync();

            var opcionAcciones = await context.OpcionAcciones
                .Where(oa => oa.OpcionId == opcion.Id)
                .ToListAsync();

            var permisos = opcionAcciones.Select(oa => new RolPermiso
            {
                RolId          = (int)RolEnum.Administrador,
                OpcionAccionId = oa.Id,
                Permitido      = true
            });

            context.RolPermisos.AddRange(permisos);
            await context.SaveChangesAsync();
        }
    }
}
