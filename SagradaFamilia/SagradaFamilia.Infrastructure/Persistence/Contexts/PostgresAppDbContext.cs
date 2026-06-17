using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace SagradaFamilia.Infrastructure.Persistence.Contexts
{
    public class PostgresAppDbContext : AppDbContext
    {
        public PostgresAppDbContext(
            DbContextOptions<PostgresAppDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null)
            : base(options, httpContextAccessor)
        {
        }
    }
}
