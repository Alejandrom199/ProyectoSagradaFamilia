using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Factories
{
    // Usada solo por "dotnet ef migrations add/update" en tiempo de diseño.
    // No afecta el comportamiento en runtime.
    public class PostgresDesignTimeFactory : IDesignTimeDbContextFactory<PostgresAppDbContext>
    {
        public PostgresAppDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<PostgresAppDbContext>()
                .UseNpgsql(
                    "Host=localhost;Port=5432;Database=SagradaFamiliaDb;Username=postgres;Password=sagrada-password",
                    npgsql => npgsql
                        .MigrationsAssembly("SagradaFamilia.Infrastructure")
                        .MigrationsHistoryTable("__EFMigrationsHistory", "public"))
                .Options;

            return new PostgresAppDbContext(options);
        }
    }
}
