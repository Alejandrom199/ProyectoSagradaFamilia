using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class NinoRepository : INinoRepository
    {
        private readonly AppDbContext _context;

        public NinoRepository(AppDbContext context) => _context = context;

        public async Task<Nino?> ObtenerPorIdAsync(int id) =>
        await _context.Ninos
            .Include(n => n.Padre)
                .ThenInclude(p => p.Usuario)
            .Include(n => n.Medico)
            .FirstOrDefaultAsync(n => n.Id == id && !n.Eliminado);

        public async Task<IEnumerable<Nino>> ObtenerTodosAsync() =>
            await _context.Ninos
                .Include(n => n.Padre)
                .Include(n => n.Medico)
                .Where(n => !n.Eliminado)
                .OrderBy(n => n.Apellido)
                .ThenBy(n => n.Nombre)
                .ToListAsync();

        public async Task<IEnumerable<Nino>> ObtenerPorPadreIdAsync(int padreId) =>
            await _context.Ninos
                .Include(n => n.Medico) 
                .Where(n => n.PadreId == padreId && !n.Eliminado)
                .OrderBy(n => n.Nombre)
                .ToListAsync();

        public async Task<IEnumerable<Nino>> ObtenerPorMedicoIdAsync(int medicoId) =>
            await _context.Ninos
                .Include(n => n.Padre)
                .Where(n => n.MedicoId == medicoId && !n.Eliminado)
                .OrderBy(n => n.Apellido)
                .ThenBy(n => n.Nombre)
                .ToListAsync();

        public async Task<(IEnumerable<Nino> Items, int TotalItems)> ObtenerPaginadoAsync(
            int page, int pageSize, string? search, string? sortBy, bool ascending, int? medicoId = null)
        {
            var query = _context.Ninos
                .Include(n => n.Padre)
                    .ThenInclude(p => p.Usuario)
                .Include(n => n.Medico)
                .Where(n => !n.Eliminado)
                .AsQueryable();

            if (medicoId.HasValue)
                query = query.Where(n => n.MedicoId == medicoId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(n =>
                    n.Nombre.ToLower().Contains(term) ||
                    n.Apellido.ToLower().Contains(term) ||
                    n.Padre.Nombre.ToLower().Contains(term) ||
                    n.Padre.Apellido.ToLower().Contains(term));
            }

            query = sortBy?.ToLower() switch
            {
                "nombre"        => ascending ? query.OrderBy(n => n.Nombre)             : query.OrderByDescending(n => n.Nombre),
                "apellido"      => ascending ? query.OrderBy(n => n.Apellido)           : query.OrderByDescending(n => n.Apellido),
                "edadmeses"     => ascending ? query.OrderBy(n => n.FechaNacimiento)    : query.OrderByDescending(n => n.FechaNacimiento),
                "nombremedio"   => ascending ? query.OrderBy(n => n.Medico.Apellido)    : query.OrderByDescending(n => n.Medico.Apellido),
                "fechacreacion" => ascending ? query.OrderBy(n => n.FechaCreacion)      : query.OrderByDescending(n => n.FechaCreacion),
                _               => query.OrderBy(n => n.Apellido).ThenBy(n => n.Nombre)
            };

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<bool> PerteneceAPadreAsync(int ninoId, int padreId) =>
            await _context.Ninos
                .AnyAsync(n => n.Id == ninoId
                            && n.PadreId == padreId
                            && !n.Eliminado);

        public async Task<bool> PerteneceAMedicoAsync(int ninoId, int medicoId) =>
            await _context.Ninos
                .AnyAsync(n => n.Id == ninoId
                            && n.MedicoId == medicoId
                            && !n.Eliminado);

        public async Task<Nino> CrearAsync(Nino nino)
        {
            _context.Ninos.Add(nino);
            await _context.SaveChangesAsync();
            return nino;
        }

        public async Task<Nino> ActualizarAsync(Nino nino)
        {
            _context.Ninos.Update(nino);
            await _context.SaveChangesAsync();
            return nino;
        }

        public async Task EliminarAsync(int id)
        {
            var nino = await ObtenerPorIdAsync(id);
            if (nino is null) return;

            _context.Ninos.Remove(nino);
            await _context.SaveChangesAsync();
        }
    }
}