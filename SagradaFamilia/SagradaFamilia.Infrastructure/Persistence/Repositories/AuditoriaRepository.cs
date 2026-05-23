using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly AppDbContext _context;

        public AuditoriaRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Auditoria>> ObtenerRecientesAsync(int top = 100) =>
            await _context.Auditorias
                .Include(a => a.Usuario)
                .OrderByDescending(a => a.Fecha)
                .Take(top)
                .ToListAsync();

        public async Task<IEnumerable<Auditoria>> ObtenerPorTablaAsync(string nombreTabla, string? clavePrimaria = null)
        {
            var query = _context.Auditorias
                .Include(a => a.Usuario)
                .Where(a => a.Tabla == nombreTabla);

            if (!string.IsNullOrWhiteSpace(clavePrimaria))
                query = query.Where(a => a.ClavePrimaria == clavePrimaria);

            return await query.OrderByDescending(a => a.Fecha).ToListAsync();
        }

        public async Task<IEnumerable<Auditoria>> ObtenerPorUsuarioAsync(int usuarioId) =>
            await _context.Auditorias
                .Include(a => a.Usuario)
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();

        public async Task RegistrarAsync(Auditoria auditoria)
        {
            _context.Auditorias.Add(auditoria);
            await _context.SaveChangesAsync();
        }
    }
}