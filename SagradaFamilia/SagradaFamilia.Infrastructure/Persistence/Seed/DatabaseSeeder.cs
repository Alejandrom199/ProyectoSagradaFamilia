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

            await AdministradorSeeder.SeedAsync(context);
            logger.LogInformation("AdministradorSeeder completado");

            await MedicoSeeder.SeedAsync(context);
            logger.LogInformation("MedicoSeeder completado");

            await MenuSeeder.SeedAsync(context);
            logger.LogInformation("MenuSeeder completado");

            await OmsSeeder.SeedAsync(context);
            logger.LogInformation("OmsSeeder completado");

            await OmsCompletaPatch.PatchAsync(context);
            logger.LogInformation("OmsCompletaPatch completado");

            await AlimentoSeeder.SeedAsync(context);
            logger.LogInformation("AlimentoSeeder completado");

            await ParametroSeeder.SeedAsync(context);
            logger.LogInformation("ParametroSeeder completado");

            await PlantillaCorreoSeeder.SeedAsync(context);
            logger.LogInformation("PlantillaCorreoSeeder completado");

            await EventoCorreoSeeder.SeedAsync(context);
            logger.LogInformation("EventoCorreoSeeder completado");

            await PlantillasMenuPatch.PatchAsync(context);
            logger.LogInformation("PlantillasMenuPatch completado");

            await EventosCorreoMenuPatch.PatchAsync(context);
            logger.LogInformation("EventosCorreoMenuPatch completado");

            logger.LogInformation("Seeds completados exitosamente.");
        }
    }
}
