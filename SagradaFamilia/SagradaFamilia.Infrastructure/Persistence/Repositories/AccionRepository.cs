using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;
using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class AccionRepository : IAccionRepository
    {
        private readonly AppDbContext _context;

        public AccionRepository(AppDbContext context) => _context = context;

        public async Task<Accion?> ObtenerPorIdAsync(int id) =>
            await _context.Acciones.FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Accion?> ObtenerPorNombreAsync(string nombre) =>
            await _context.Acciones
                .FirstOrDefaultAsync(a => a.Nombre.ToLower() == nombre.ToLower());

        public async Task<IEnumerable<Accion>> ObtenerTodosAsync() =>
            await _context.Acciones.OrderBy(a => a.Nombre).ToListAsync();
    }
}
