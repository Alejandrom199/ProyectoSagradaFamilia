using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Infrastructure.Logging;
using SagradaFamilia.Infrastructure.Reporting;

namespace SagradaFamilia.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IReportesService, ReportesService>();

            // Registro del logger que persiste Warning/Error en la BD
            services.AddSingleton<ILoggerProvider, DatabaseLoggerProvider>();

            return services;
        }
    }
}
