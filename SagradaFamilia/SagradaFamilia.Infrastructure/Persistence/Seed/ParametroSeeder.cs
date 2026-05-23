using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class ParametroSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.ParametrosSistema.AnyAsync())
                return;

            var parametros = new List<ParametroSistema>
            {
                new()
                {
                    Grupo = "HORARIO_ATENCION",
                    Codigo = "HORA_INICIO",
                    Valor = "08:00",
                    Descripcion = "Hora de inicio de atención médica (formato HH:mm)",
                    Activo = true
                },
                new()
                {
                    Grupo = "HORARIO_ATENCION",
                    Codigo = "HORA_FIN",
                    Valor = "18:00",
                    Descripcion = "Hora de fin de atención médica (formato HH:mm)",
                    Activo = true
                },
                new()
                {
                    Grupo = "HORARIO_ATENCION",
                    Codigo = "DIAS_HABILES",
                    Valor = "1,2,3,4,5",
                    Descripcion = "Días hábiles de atención (1=Lunes, 2=Martes, ..., 7=Domingo)",
                    Activo = true
                }
            };

            context.ParametrosSistema.AddRange(parametros);
            await context.SaveChangesAsync();
        }
    }
}
