using Microsoft.Extensions.Logging;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly AppDbContext _context;

        public DatabaseLogger(string categoryName, AppDbContext context)
        {
            _categoryName = categoryName;
            _context = context;
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

            // Ignorar logs internos de EF Core para no saturar la tabla
            if (_categoryName.StartsWith("Microsoft.EntityFrameworkCore"))
                return;

            var mensaje = formatter(state, exception);

            try
            {
                _context.LogsSistema.Add(new Domain.Entities.LogSistema
                {
                    FechaHora = DateTime.UtcNow,
                    Nivel = logLevel.ToString(),
                    Mensaje = mensaje.Length > 2000 ? mensaje[..2000] : mensaje,
                    Excepcion = exception?.Message,
                    StackTrace = exception?.StackTrace?[..Math.Min(exception.StackTrace?.Length ?? 0, 4000)]
                });

                _context.SaveChanges();
            }
            catch
            {
                // Si falla el log en BD no debe romper la aplicación
            }
        }
    }
}
