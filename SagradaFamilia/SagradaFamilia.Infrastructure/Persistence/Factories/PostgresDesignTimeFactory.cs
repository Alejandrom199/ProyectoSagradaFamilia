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
            // Lee la conexión de la variable de entorno para no exponer credenciales en el repo.
            // Setear antes de correr "dotnet ef migrations ...":
            //   $env:POSTGRES_CONNECTION="Host=localhost;Port=5432;Database=...;Username=postgres;Password=..."
            var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
                ?? "Host=localhost;Port=5432;Database=SagradaFamiliaDb;Username=postgres;Password=postgres";

            var options = new DbContextOptionsBuilder<PostgresAppDbContext>()
                .UseNpgsql(
                    connectionString,
                    npgsql => npgsql
                        .MigrationsAssembly("SagradaFamilia.Infrastructure")
                        .MigrationsHistoryTable("__EFMigrationsHistory", "public"))
                .Options;

            return new PostgresAppDbContext(options);
        }
    }
}
