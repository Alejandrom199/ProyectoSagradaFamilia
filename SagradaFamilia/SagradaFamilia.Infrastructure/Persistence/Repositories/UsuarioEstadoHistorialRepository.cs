using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class UsuarioEstadoHistorialRepository : IUsuarioEstadoHistorialRepository
    {
        private readonly AppDbContext _context;

        public UsuarioEstadoHistorialRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<UsuarioEstadoHistorial>> ObtenerPorUsuarioAsync(int usuarioId) =>
            await _context.UsuarioEstadoHistorial
                .Include(h => h.UsuarioQueRealizoCambio)
                .Where(h => h.UsuarioId == usuarioId)
                .OrderByDescending(h => h.FechaCambio)
                .ToListAsync();

        public async Task RegistrarAsync(UsuarioEstadoHistorial registro)
        {
            _context.UsuarioEstadoHistorial.Add(registro);
            await _context.SaveChangesAsync();
        }
    }
}
