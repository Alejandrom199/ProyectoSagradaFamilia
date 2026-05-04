using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly AppDbContext _context;

        public RolRepository(AppDbContext context) => _context = context;

        public async Task<Rol?> ObtenerPorIdAsync(int id) =>
            await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Rol?> ObtenerPorNombreAsync(string nombre) =>
            await _context.Roles
                .FirstOrDefaultAsync(r => r.Nombre.ToLower() == nombre.ToLower());

        public async Task<IEnumerable<Rol>> ObtenerTodosAsync() =>
            await _context.Roles.OrderBy(r => r.Nombre).ToListAsync();
    }
}
