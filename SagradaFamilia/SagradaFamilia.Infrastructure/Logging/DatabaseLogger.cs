using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;
using System.Security.Claims;

namespace SagradaFamilia.Infrastructure.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DatabaseLogger(
            string categoryName,
            IServiceScopeFactory scopeFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _categoryName = categoryName;
            _scopeFactory = scopeFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        // Solo Warning y Error van a BD — Information es ruido de consola/dev
        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            // Evitar recursión con los logs internos de EF Core y HTTP
            if (_categoryName.StartsWith("Microsoft.EntityFrameworkCore") ||
                _categoryName.StartsWith("System.Net.Http") ||
                _categoryName.StartsWith("Microsoft.AspNetCore")) return;

            var mensaje = formatter(state, exception);
            var httpContext = _httpContextAccessor.HttpContext;

            var endpoint = httpContext?.Request.Path.Value ?? "Background";
            var userIdClaim = httpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? usuarioId = int.TryParse(userIdClaim, out var uid) ? uid : null;

            using var scope = _scopeFactory.CreateScope();
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
                    Endpoint = endpoint,
                    UsuarioId = usuarioId
                });

                context.SaveChanges();
            }
            catch { /* un logger nunca debe romper el flujo principal */ }
        }
    }
}
