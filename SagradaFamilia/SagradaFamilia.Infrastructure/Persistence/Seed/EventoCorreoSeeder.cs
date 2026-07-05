using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Crea los eventos de correo del sistema y los vincula a las plantillas existentes.
    /// Idempotente: solo se ejecuta si no hay eventos registrados.
    /// </summary>
    public static class EventoCorreoSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var plantillas = await context.PlantillasCorreo
                .Where(p => !p.Eliminado)
                .ToDictionaryAsync(p => p.Codigo ?? string.Empty, p => p.Id);

            int? IdPlantilla(string codigo) =>
                plantillas.TryGetValue(codigo, out var id) ? id : null;

            var existentes = (await context.EventosCorreo
                .Select(e => e.Codigo)
                .ToListAsync())
                .ToHashSet();

            var candidatos = new List<EventoCorreo>
            {
                new()
                {
                    Codigo            = "CUENTA_PADRE",
                    Nombre            = "Activación de cuenta — Padre",
                    Descripcion       = "Se dispara cuando se registra un nuevo padre o representante. El destinatario recibe un enlace para establecer su contraseña y activar su cuenta.",
                    Variables         = """["NOMBRE","APELLIDO","LINK"]""",
                    PlantillaCorreoId = IdPlantilla("CUENTA_PADRE")
                },
                new()
                {
                    Codigo            = "CUENTA_MEDICO",
                    Nombre            = "Activación de cuenta — Médico",
                    Descripcion       = "Se dispara cuando el administrador registra un nuevo médico. El médico recibe un enlace para activar su cuenta y establecer su contraseña.",
                    Variables         = """["NOMBRE","APELLIDO","LINK"]""",
                    PlantillaCorreoId = IdPlantilla("CUENTA_MEDICO")
                },
                new()
                {
                    Codigo            = "CUENTA_ADMIN",
                    Nombre            = "Activación de cuenta — Administrador",
                    Descripcion       = "Se dispara cuando se registra un nuevo administrador. El destinatario recibe un enlace para activar su acceso al sistema.",
                    Variables         = """["EMAIL","LINK"]""",
                    PlantillaCorreoId = IdPlantilla("CUENTA_ADMIN")
                },
                new()
                {
                    Codigo            = "CAMBIO_CLAVE",
                    Nombre            = "Restablecimiento de contraseña",
                    Descripcion       = "Se dispara cuando un médico o padre solicita restablecer su contraseña. El destinatario recibe un enlace de recuperación válido por 24 horas.",
                    Variables         = """["NOMBRE","LINK"]""",
                    PlantillaCorreoId = IdPlantilla("CAMBIO_CLAVE")
                },
                new()
                {
                    Codigo            = "CITA_AGENDADA",
                    Nombre            = "Cita agendada — Notificación al padre",
                    Descripcion       = "Se dispara cuando se programa una nueva cita. El padre recibe un correo con los datos de la consulta.",
                    Variables         = """["NOMBRE_PADRE","NOMBRE_NINO","FECHA","HORA_INICIO","HORA_FIN","MEDICO","MOTIVO"]""",
                    PlantillaCorreoId = IdPlantilla("CITA_AGENDADA")
                },
                new()
                {
                    Codigo            = "CITA_REAGENDADA",
                    Nombre            = "Cita reagendada — Notificación al padre",
                    Descripcion       = "Se dispara cuando una cita es reprogramada. El padre recibe la nueva fecha y hora confirmada.",
                    Variables         = """["NOMBRE_PADRE","NOMBRE_NINO","FECHA","HORA_INICIO","HORA_FIN","MEDICO"]""",
                    PlantillaCorreoId = IdPlantilla("CITA_REAGENDADA")
                },
            };

            var nuevos = candidatos.Where(e => !existentes.Contains(e.Codigo)).ToList();
            if (nuevos.Count == 0) return;

            context.EventosCorreo.AddRange(nuevos);
            await context.SaveChangesAsync();
        }
    }
}
