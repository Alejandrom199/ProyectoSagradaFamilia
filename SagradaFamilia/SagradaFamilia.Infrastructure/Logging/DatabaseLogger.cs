using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IServiceScopeFactory _scopeFactory;

        public DatabaseLogger(string categoryName, IServiceScopeFactory scopeFactory)
        {
            _categoryName = categoryName;
            _scopeFactory = scopeFactory;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            => null;

        public bool IsEnabled(LogLevel logLevel) =>
            logLevel >= LogLevel.Information;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (_categoryName.StartsWith("Microsoft.EntityFrameworkCore") ||
                _categoryName.StartsWith("System.Net.Http")) return;

            var mensaje = formatter(state, exception);

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                try
                {
                    context.LogsSistema.Add(new LogSistema
                    {
                        FechaHora = DateTime.UtcNow,
                        Nivel = logLevel.ToString(),
                        Mensaje = mensaje.Length > 2000 ? mensaje[..2000] : mensaje,
                        Excepcion = exception?.Message,
                        StackTrace = exception?.StackTrace?[..Math.Min(exception.StackTrace?.Length ?? 0, 4000)],
                        Endpoint = "Global"
                    });

                    context.SaveChanges();
                }
                catch { /* un logger nunca debe romper el flujo principal */ }
            }
        }
    }
}
