using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Detecta si los permisos de rol están desactualizados (p.ej. el Administrador
    /// tiene acceso a secciones clínicas que ya no le corresponden) y los restablece
    /// desde cero con la lógica correcta del MenuSeeder actual.
    ///
    /// Condición de disparo: Admin tiene permiso sobre la ruta /pacientes.
    /// Si ya está correcto, el patch no hace nada (idempotente).
    /// </summary>
    public static class RolPermisosResetPatch
    {
        public static async Task PatchAsync(AppDbContext context)
        {
            // Si el admin ya NO tiene acceso a /pacientes → permisos ya están bien
            var adminTieneRutaIncorrecta = await context.RolPermisos
                .Include(rp => rp.OpcionAccion)
                    .ThenInclude(oa => oa.Opcion)
                .AnyAsync(rp => rp.RolId == (int)RolEnum.Administrador
                             && rp.Permitido
                             && rp.OpcionAccion.Opcion.Ruta == "/pacientes");

            if (!adminTieneRutaIncorrecta) return;

            // Borrar TODOS los permisos de rol y re-sembrar correctamente
            var todos = await context.RolPermisos.ToListAsync();
            context.RolPermisos.RemoveRange(todos);
            await context.SaveChangesAsync();

            await SembrarPermisosAsync(context);
        }

        private static async Task SembrarPermisosAsync(AppDbContext context)
        {
            int idAdmin  = (int)RolEnum.Administrador;
            int idMedico = (int)RolEnum.Medico;
            int idPadre  = (int)RolEnum.Padre;

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

                // Administrador: dashboard + gestión del sistema + monitoreo (sin clínica)
                bool adminTieneAcceso =
                    modulo == "Principal"      ||
                    modulo == "Administración" ||
                    modulo == "Monitoreo";

                if (adminTieneAcceso)
                    rolPermisos.Add(new RolPermiso { RolId = idAdmin, OpcionAccionId = oa.Id, Permitido = true });

                // Médico: dashboard + secciones clínicas + padres + monitoreo básico
                bool medicoTieneAcceso =
                    modulo == "Principal"      ||
                    (modulo == "Administración" && opcion == "Padres de Familia") ||
                    modulo == "Gestión de Pacientes" ||
                    modulo == "Atención Médica" ||
                    modulo == "Predicciones" ||
                    modulo == "Orientación Alimentaria" ||
                    (modulo == "Monitoreo" && opcion == "Actividad del Sistema");

                if (medicoTieneAcceso)
                    rolPermisos.Add(new RolPermiso { RolId = idMedico, OpcionAccionId = oa.Id, Permitido = true });

                // Padre: solo ver pacientes
                if (modulo == "Gestión de Pacientes" && accion == "Ver")
                    rolPermisos.Add(new RolPermiso { RolId = idPadre, OpcionAccionId = oa.Id, Permitido = true });
            }

            context.RolPermisos.AddRange(rolPermisos);
            await context.SaveChangesAsync();
        }
    }
}
