using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Logging
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DatabaseLoggerProvider(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public ILogger CreateLogger(string categoryName) =>
        new DatabaseLogger(categoryName, _scopeFactory);

        public void Dispose() { }
    }
}
