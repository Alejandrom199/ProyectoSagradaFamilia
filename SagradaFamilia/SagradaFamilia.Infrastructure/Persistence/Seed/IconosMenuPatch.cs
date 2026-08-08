using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Reasigna íconos de Módulos/Opciones ya sembrados a valores más representativos
    /// (agregados tras incorporar Font Awesome al catálogo de íconos del frontend).
    /// Idempotente: vuelve a asignar el mismo valor si ya se aplicó, sin efectos secundarios.
    /// </summary>
    public static class IconosMenuPatch
    {
        public static async Task PatchAsync(AppDbContext context)
        {
            var iconosPorModulo = new Dictionary<string, string>
            {
                ["Atención Médica"] = "notes-medical",
                ["Predicciones"]    = "brain",
            };

            var iconosPorOpcionRuta = new Dictionary<string, string>
            {
                ["/medicos"]            = "user-doctor",
                ["/parametros"]         = "tune",
                ["/citas"]              = "calendar-check",
                ["/citas/historial"]    = "history",
                ["/prescripciones"]     = "file-prescription",
                ["/predicciones"]       = "chart-line",
                ["/alimentos"]          = "utensils",
                ["/sistema/eventos-correo"] = "envelope-check",
            };

            var modulos = await context.Modulos
                .Where(m => iconosPorModulo.Keys.Contains(m.Nombre))
                .ToListAsync();

            foreach (var modulo in modulos)
                modulo.Icono = iconosPorModulo[modulo.Nombre];

            var opciones = await context.Opciones
                .Where(o => o.Ruta != null && iconosPorOpcionRuta.Keys.Contains(o.Ruta))
                .ToListAsync();

            foreach (var opcion in opciones)
                opcion.Icono = iconosPorOpcionRuta[opcion.Ruta!];

            if (modulos.Count > 0 || opciones.Count > 0)
                await context.SaveChangesAsync();
        }
    }
}
