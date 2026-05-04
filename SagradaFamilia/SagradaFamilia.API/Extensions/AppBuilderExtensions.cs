using Microsoft.EntityFrameworkCore;
using SagradaFamilia.API.Middlewares;
using SagradaFamilia.Infrastructure.Logging;
using SagradaFamilia.Infrastructure.Persistence.Contexts;
using SagradaFamilia.Infrastructure.Persistence.Seed;

namespace SagradaFamilia.API.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sagrada Familia API v1");
                options.RoutePrefix = "swagger";
            });
            return app;
        }

        public static async Task ApplyMigrationsAndSeedsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            // Registrar el logger de BD
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(new DatabaseLoggerProvider(context));

            logger.LogInformation("Aplicando migraciones pendientes...");
            await context.Database.MigrateAsync();

            logger.LogInformation("Ejecutando seeds...");
            await DatabaseSeeder.SeedAllAsync(context, logger);
        }
    }
}
