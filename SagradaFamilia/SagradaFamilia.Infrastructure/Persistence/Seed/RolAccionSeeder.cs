using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class RolAccionSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles ON");
                    context.Roles.AddRange(
                        new Rol { Id = 1, Nombre = "Medico" },
                        new Rol { Id = 2, Nombre = "Padre" }
                    );
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Roles OFF");
                    await transaction.CommitAsync();
                }
                catch { await transaction.RollbackAsync(); throw; }
            }

            if (!await context.Acciones.AnyAsync())
            {
                var acciones = Enum.GetNames(typeof(Domain.Enums.Accion))
                    .Select(nombre => new Domain.Entities.Accion { Nombre = nombre });

                context.Acciones.AddRange(acciones);
                await context.SaveChangesAsync();
            }
        }
    }
}
