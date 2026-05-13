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

        public async Task<IEnumerable<Auditoria>> ObtenerPorTablaAsync(string nombreTabla, string clavePrimaria) =>
            await _context.Auditorias
                .Include(a => a.Usuario) 
                .Where(a => a.Tabla == nombreTabla && a.ClavePrimaria == clavePrimaria)
                .OrderByDescending(a => a.Fecha) 
                .ToListAsync();

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