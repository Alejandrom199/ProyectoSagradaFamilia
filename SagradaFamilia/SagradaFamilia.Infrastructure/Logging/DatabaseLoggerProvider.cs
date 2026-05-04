using Microsoft.Extensions.Logging;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Logging
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly AppDbContext _context;

        public DatabaseLoggerProvider(AppDbContext context)
        {
            _context = context;
        }

        public ILogger CreateLogger(string categoryName) =>
            new DatabaseLogger(categoryName, _context);

        public void Dispose() { }
    }
}
