using Microsoft.Extensions.Logging;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAllAsync(AppDbContext context, ILogger logger)
        {
            logger.LogInformation("Iniciando seeds...");

            // Primero roles y acciones — los demás dependen de estos
            await RolAccionSeeder.SeedAsync(context);
            logger.LogInformation("RolAccionSeeder completado");

            await MedicoSeeder.SeedAsync(context);
            logger.LogInformation("MedicoSeeder completado");

            await MenuSeeder.SeedAsync(context);
            logger.LogInformation("MenuSeeder completado");

            await OmsSeeder.SeedAsync(context);
            logger.LogInformation("OmsSeeder completado");

            await AlimentoSeeder.SeedAsync(context);
            logger.LogInformation("AlimentoSeeder completado");

            logger.LogInformation("Seeds completados exitosamente.");
        }
    }
}
