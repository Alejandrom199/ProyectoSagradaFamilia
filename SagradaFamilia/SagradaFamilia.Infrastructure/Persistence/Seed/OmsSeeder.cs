using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class OmsSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.OmsPesoPorEdad.AnyAsync())
                return;

            var datos = new List<OmsPesoPorEdad>
        {
            // ── Varones ──────────────────────────────────────────────────
            new() { Sexo = 'M', EdadMeses = 0,  Percentil3 = 2.5m,  Percentil15 = 2.9m,  Percentil50 = 3.3m,  Percentil85 = 3.9m,  Percentil97 = 4.4m  },
            new() { Sexo = 'M', EdadMeses = 1,  Percentil3 = 3.4m,  Percentil15 = 3.9m,  Percentil50 = 4.5m,  Percentil85 = 5.1m,  Percentil97 = 5.7m  },
            new() { Sexo = 'M', EdadMeses = 2,  Percentil3 = 4.3m,  Percentil15 = 4.9m,  Percentil50 = 5.6m,  Percentil85 = 6.3m,  Percentil97 = 7.1m  },
            new() { Sexo = 'M', EdadMeses = 3,  Percentil3 = 5.0m,  Percentil15 = 5.7m,  Percentil50 = 6.4m,  Percentil85 = 7.2m,  Percentil97 = 8.0m  },
            new() { Sexo = 'M', EdadMeses = 4,  Percentil3 = 5.6m,  Percentil15 = 6.2m,  Percentil50 = 7.0m,  Percentil85 = 7.8m,  Percentil97 = 8.7m  },
            new() { Sexo = 'M', EdadMeses = 5,  Percentil3 = 6.0m,  Percentil15 = 6.7m,  Percentil50 = 7.5m,  Percentil85 = 8.4m,  Percentil97 = 9.3m  },
            new() { Sexo = 'M', EdadMeses = 6,  Percentil3 = 6.4m,  Percentil15 = 7.1m,  Percentil50 = 7.9m,  Percentil85 = 8.8m,  Percentil97 = 9.8m  },
            new() { Sexo = 'M', EdadMeses = 7,  Percentil3 = 6.7m,  Percentil15 = 7.4m,  Percentil50 = 8.3m,  Percentil85 = 9.2m,  Percentil97 = 10.3m },
            new() { Sexo = 'M', EdadMeses = 8,  Percentil3 = 6.9m,  Percentil15 = 7.7m,  Percentil50 = 8.6m,  Percentil85 = 9.6m,  Percentil97 = 10.7m },
            new() { Sexo = 'M', EdadMeses = 9,  Percentil3 = 7.1m,  Percentil15 = 7.9m,  Percentil50 = 8.9m,  Percentil85 = 9.9m,  Percentil97 = 11.0m },
            new() { Sexo = 'M', EdadMeses = 10, Percentil3 = 7.4m,  Percentil15 = 8.2m,  Percentil50 = 9.2m,  Percentil85 = 10.2m, Percentil97 = 11.4m },
            new() { Sexo = 'M', EdadMeses = 11, Percentil3 = 7.6m,  Percentil15 = 8.4m,  Percentil50 = 9.4m,  Percentil85 = 10.5m, Percentil97 = 11.7m },
            new() { Sexo = 'M', EdadMeses = 12, Percentil3 = 7.7m,  Percentil15 = 8.6m,  Percentil50 = 9.6m,  Percentil85 = 10.8m, Percentil97 = 12.0m },
            new() { Sexo = 'M', EdadMeses = 15, Percentil3 = 8.1m,  Percentil15 = 9.0m,  Percentil50 = 10.1m, Percentil85 = 11.3m, Percentil97 = 12.7m },
            new() { Sexo = 'M', EdadMeses = 18, Percentil3 = 8.4m,  Percentil15 = 9.4m,  Percentil50 = 10.6m, Percentil85 = 11.9m, Percentil97 = 13.4m },
            new() { Sexo = 'M', EdadMeses = 21, Percentil3 = 8.8m,  Percentil15 = 9.8m,  Percentil50 = 11.0m, Percentil85 = 12.4m, Percentil97 = 13.9m },
            new() { Sexo = 'M', EdadMeses = 24, Percentil3 = 9.2m,  Percentil15 = 10.2m, Percentil50 = 11.5m, Percentil85 = 12.9m, Percentil97 = 14.5m },
            new() { Sexo = 'M', EdadMeses = 30, Percentil3 = 9.9m,  Percentil15 = 11.0m, Percentil50 = 12.4m, Percentil85 = 14.0m, Percentil97 = 15.7m },
            new() { Sexo = 'M', EdadMeses = 36, Percentil3 = 10.6m, Percentil15 = 11.8m, Percentil50 = 13.3m, Percentil85 = 15.0m, Percentil97 = 16.9m },
            new() { Sexo = 'M', EdadMeses = 42, Percentil3 = 11.2m, Percentil15 = 12.5m, Percentil50 = 14.1m, Percentil85 = 16.0m, Percentil97 = 18.1m },
            new() { Sexo = 'M', EdadMeses = 48, Percentil3 = 11.8m, Percentil15 = 13.2m, Percentil50 = 15.0m, Percentil85 = 17.0m, Percentil97 = 19.3m },
            new() { Sexo = 'M', EdadMeses = 54, Percentil3 = 12.4m, Percentil15 = 13.9m, Percentil50 = 15.8m, Percentil85 = 18.0m, Percentil97 = 20.5m },
            new() { Sexo = 'M', EdadMeses = 60, Percentil3 = 13.0m, Percentil15 = 14.6m, Percentil50 = 16.7m, Percentil85 = 19.1m, Percentil97 = 21.8m },

            // ── Niñas ─────────────────────────────────────────────────────
            new() { Sexo = 'F', EdadMeses = 0,  Percentil3 = 2.4m,  Percentil15 = 2.8m,  Percentil50 = 3.2m,  Percentil85 = 3.7m,  Percentil97 = 4.2m  },
            new() { Sexo = 'F', EdadMeses = 1,  Percentil3 = 3.2m,  Percentil15 = 3.6m,  Percentil50 = 4.2m,  Percentil85 = 4.8m,  Percentil97 = 5.5m  },
            new() { Sexo = 'F', EdadMeses = 2,  Percentil3 = 3.9m,  Percentil15 = 4.5m,  Percentil50 = 5.1m,  Percentil85 = 5.8m,  Percentil97 = 6.6m  },
            new() { Sexo = 'F', EdadMeses = 3,  Percentil3 = 4.5m,  Percentil15 = 5.2m,  Percentil50 = 5.8m,  Percentil85 = 6.6m,  Percentil97 = 7.5m  },
            new() { Sexo = 'F', EdadMeses = 4,  Percentil3 = 5.0m,  Percentil15 = 5.7m,  Percentil50 = 6.4m,  Percentil85 = 7.3m,  Percentil97 = 8.2m  },
            new() { Sexo = 'F', EdadMeses = 5,  Percentil3 = 5.4m,  Percentil15 = 6.1m,  Percentil50 = 6.9m,  Percentil85 = 7.8m,  Percentil97 = 8.8m  },
            new() { Sexo = 'F', EdadMeses = 6,  Percentil3 = 5.7m,  Percentil15 = 6.5m,  Percentil50 = 7.3m,  Percentil85 = 8.2m,  Percentil97 = 9.3m  },
            new() { Sexo = 'F', EdadMeses = 7,  Percentil3 = 6.0m,  Percentil15 = 6.8m,  Percentil50 = 7.6m,  Percentil85 = 8.6m,  Percentil97 = 9.7m  },
            new() { Sexo = 'F', EdadMeses = 8,  Percentil3 = 6.3m,  Percentil15 = 7.0m,  Percentil50 = 7.9m,  Percentil85 = 9.0m,  Percentil97 = 10.2m },
            new() { Sexo = 'F', EdadMeses = 9,  Percentil3 = 6.5m,  Percentil15 = 7.3m,  Percentil50 = 8.2m,  Percentil85 = 9.3m,  Percentil97 = 10.5m },
            new() { Sexo = 'F', EdadMeses = 10, Percentil3 = 6.7m,  Percentil15 = 7.5m,  Percentil50 = 8.5m,  Percentil85 = 9.6m,  Percentil97 = 10.9m },
            new() { Sexo = 'F', EdadMeses = 11, Percentil3 = 6.9m,  Percentil15 = 7.7m,  Percentil50 = 8.7m,  Percentil85 = 9.9m,  Percentil97 = 11.2m },
            new() { Sexo = 'F', EdadMeses = 12, Percentil3 = 7.0m,  Percentil15 = 7.9m,  Percentil50 = 8.9m,  Percentil85 = 10.1m, Percentil97 = 11.5m },
            new() { Sexo = 'F', EdadMeses = 15, Percentil3 = 7.4m,  Percentil15 = 8.3m,  Percentil50 = 9.4m,  Percentil85 = 10.7m, Percentil97 = 12.2m },
            new() { Sexo = 'F', EdadMeses = 18, Percentil3 = 7.8m,  Percentil15 = 8.8m,  Percentil50 = 9.9m,  Percentil85 = 11.3m, Percentil97 = 12.9m },
            new() { Sexo = 'F', EdadMeses = 21, Percentil3 = 8.2m,  Percentil15 = 9.2m,  Percentil50 = 10.4m, Percentil85 = 11.8m, Percentil97 = 13.5m },
            new() { Sexo = 'F', EdadMeses = 24, Percentil3 = 8.6m,  Percentil15 = 9.6m,  Percentil50 = 10.9m, Percentil85 = 12.4m, Percentil97 = 14.2m },
            new() { Sexo = 'F', EdadMeses = 30, Percentil3 = 9.3m,  Percentil15 = 10.4m, Percentil50 = 11.8m, Percentil85 = 13.5m, Percentil97 = 15.5m },
            new() { Sexo = 'F', EdadMeses = 36, Percentil3 = 10.0m, Percentil15 = 11.2m, Percentil50 = 12.7m, Percentil85 = 14.6m, Percentil97 = 16.8m },
            new() { Sexo = 'F', EdadMeses = 42, Percentil3 = 10.7m, Percentil15 = 12.0m, Percentil50 = 13.7m, Percentil85 = 15.7m, Percentil97 = 18.1m },
            new() { Sexo = 'F', EdadMeses = 48, Percentil3 = 11.3m, Percentil15 = 12.7m, Percentil50 = 14.5m, Percentil85 = 16.7m, Percentil97 = 19.3m },
            new() { Sexo = 'F', EdadMeses = 54, Percentil3 = 11.9m, Percentil15 = 13.4m, Percentil50 = 15.4m, Percentil85 = 17.8m, Percentil97 = 20.6m },
            new() { Sexo = 'F', EdadMeses = 60, Percentil3 = 12.5m, Percentil15 = 14.1m, Percentil50 = 16.2m, Percentil85 = 18.9m, Percentil97 = 21.9m },
        };

            context.OmsPesoPorEdad.AddRange(datos);
            await context.SaveChangesAsync();
        }
    }
}
