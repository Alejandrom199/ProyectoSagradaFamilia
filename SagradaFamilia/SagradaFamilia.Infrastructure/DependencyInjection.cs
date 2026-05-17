using Microsoft.Extensions.DependencyInjection;
using SagradaFamilia.Application.Interfaces.Services;
using SagradaFamilia.Infrastructure.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagradaFamilia.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IReportesService, ReportesService>();

            return services;
        }
    }
}
