using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class PadreRepository : IPadreRepository
    {
        private readonly AppDbContext _context;

        public PadreRepository(AppDbContext context) => _context = context;

        public async Task<Padre?> ObtenerPorIdAsync(int id) =>
            await _context.Padres
                .Include(p => p.Usuario)
                .Include(p => p.Medico) 
                .Include(p => p.Ninos.Where(n => !n.Eliminado)) 
                .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado);

        public async Task<Padre?> ObtenerPorUsuarioIdAsync(int usuarioId) =>
            await _context.Padres
                .Include(p => p.Usuario)
                .Include(p => p.Medico)
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && !p.Eliminado);

        public async Task<IEnumerable<Padre>> ObtenerTodosAsync() =>
            await _context.Padres
                .Include(p => p.Usuario)
                .Include(p => p.Medico)
                .Include(p => p.Ninos)
                .Where(p => !p.Eliminado)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToListAsync();

        public async Task<(IEnumerable<Padre> Items, int TotalItems)> ObtenerPaginadoAsync(
            int page, int pageSize, string? search, string? sortBy, bool ascending)
        {
            var query = _context.Padres
                .Include(p => p.Usuario)
                .Include(p => p.Medico)
                .Include(p => p.Ninos.Where(n => !n.Eliminado))
                .Where(p => !p.Eliminado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(p =>
                    p.Nombre.ToLower().Contains(term) ||
                    p.Apellido.ToLower().Contains(term) ||
                    p.Usuario.Email.ToLower().Contains(term));
            }

            query = sortBy?.ToLower() switch
            {
                "nombre"        => ascending ? query.OrderBy(p => p.Nombre)              : query.OrderByDescending(p => p.Nombre),
                "apellido"      => ascending ? query.OrderBy(p => p.Apellido)            : query.OrderByDescending(p => p.Apellido),
                "email"         => ascending ? query.OrderBy(p => p.Usuario.Email)       : query.OrderByDescending(p => p.Usuario.Email),
                "fechacreacion" => ascending ? query.OrderBy(p => p.FechaCreacion)       : query.OrderByDescending(p => p.FechaCreacion),
                _               => query.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
            };

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<IEnumerable<Padre>> ObtenerPorMedicoIdAsync(int medicoId) =>
            await _context.Padres
                .Where(p => p.MedicoId == medicoId && !p.Eliminado)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToListAsync();

        public async Task<Padre> CrearAsync(Padre padre)
        {
            _context.Padres.Add(padre);
            await _context.SaveChangesAsync();
            return padre;
        }

        public async Task<Padre> ActualizarAsync(Padre padre)
        {
            _context.Padres.Update(padre);
            await _context.SaveChangesAsync();
            return padre;
        }

        public async Task EliminarAsync(int id)
        {
            var padre = await ObtenerPorIdAsync(id);
            if (padre is null) return;

            padre.Eliminado = true;
            padre.FechaEliminacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}