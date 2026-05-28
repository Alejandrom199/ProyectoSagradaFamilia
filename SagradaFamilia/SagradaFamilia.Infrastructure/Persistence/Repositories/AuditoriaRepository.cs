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

        public async Task<(IEnumerable<Auditoria> Items, int TotalItems)> ObtenerPaginadoAsync(
            int page, int pageSize, string? search, string? sortBy, bool ascending)
        {
            var query = _context.Auditorias
                .Include(a => a.Usuario)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(a =>
                    a.Tabla.ToLower().Contains(term) ||
                    a.Accion.ToLower().Contains(term) ||
                    a.ClavePrimaria.ToLower().Contains(term) ||
                    (a.Usuario != null && a.Usuario.Email.ToLower().Contains(term)));
            }

            query = sortBy?.ToLower() switch
            {
                "fecha"       => ascending ? query.OrderBy(a => a.Fecha)           : query.OrderByDescending(a => a.Fecha),
                "accion"      => ascending ? query.OrderBy(a => a.Accion)          : query.OrderByDescending(a => a.Accion),
                "tabla"       => ascending ? query.OrderBy(a => a.Tabla)           : query.OrderByDescending(a => a.Tabla),
                "usuarioemail"=> ascending ? query.OrderBy(a => a.Usuario!.Email)  : query.OrderByDescending(a => a.Usuario!.Email),
                _             => query.OrderByDescending(a => a.Fecha)
            };

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task RegistrarAsync(Auditoria auditoria)
        {
            _context.Auditorias.Add(auditoria);
            await _context.SaveChangesAsync();
        }
    }
}