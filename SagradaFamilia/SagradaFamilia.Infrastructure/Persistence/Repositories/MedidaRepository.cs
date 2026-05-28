using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class MedidaRepository : IMedidaRepository
    {
        private readonly AppDbContext _context;

        public MedidaRepository(AppDbContext context) => _context = context;

        public async Task<Medida?> ObtenerPorIdAsync(int id) =>
            await _context.Medidas
                .Include(m => m.Nino)
                .Include(m => m.Medico)
                .FirstOrDefaultAsync(m => m.Id == id && !m.Eliminado);

        public async Task<IEnumerable<Medida>> ObtenerPorNinoAsync(int ninoId) =>
            await _context.Medidas
                .Include(m => m.Medico)
                .Where(m => m.NinoId == ninoId && !m.Eliminado)
                .OrderByDescending(m => m.FechaMedicion)
                .ToListAsync();

        public async Task<Medida?> ObtenerUltimaMedidaAsync(int ninoId) =>
            await _context.Medidas
                .Include(m => m.Medico)
                .Where(m => m.NinoId == ninoId && !m.Eliminado)
                .OrderByDescending(m => m.FechaMedicion)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<Medida>> ObtenerTodosAsync() =>
            await _context.Medidas
                .Include(m => m.Nino)
                .Include(m => m.Medico)
                .Where(m => !m.Eliminado)
                .OrderByDescending(m => m.FechaMedicion)
                .ToListAsync();

        public async Task<Medida?> ObtenerPorNinoYMesAsync(int ninoId, int mes, int anio) =>
            await _context.Medidas
                .FirstOrDefaultAsync(m => m.NinoId == ninoId
                                       && !m.Eliminado
                                       && m.FechaMedicion.Year == anio
                                       && m.FechaMedicion.Month == mes);

        public async Task<bool> ExisteMedidaEnMesAsync(int ninoId, int mes, int anio) =>
            await _context.Medidas
                .AnyAsync(m => m.NinoId == ninoId
                            && !m.Eliminado
                            && m.FechaMedicion.Year == anio
                            && m.FechaMedicion.Month == mes);

        public async Task<(IEnumerable<Medida> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(
            int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending)
        {
            var query = _context.Medidas
                .Include(m => m.Medico)
                .Where(m => m.NinoId == ninoId && !m.Eliminado)
                .AsQueryable();

            query = sortBy?.ToLower() switch
            {
                "peso"  => ascending ? query.OrderBy(m => m.Peso)  : query.OrderByDescending(m => m.Peso),
                "talla" => ascending ? query.OrderBy(m => m.Talla) : query.OrderByDescending(m => m.Talla),
                _       => ascending ? query.OrderBy(m => m.FechaMedicion) : query.OrderByDescending(m => m.FechaMedicion)
            };

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<Medida> CrearAsync(Medida medida)
        {
            _context.Medidas.Add(medida);
            await _context.SaveChangesAsync();
            return medida;
        }

        public async Task<Medida> ActualizarAsync(Medida medida)
        {
            _context.Medidas.Update(medida);
            await _context.SaveChangesAsync();
            return medida;
        }

        public async Task EliminarAsync(int id)
        {
            var medida = await ObtenerPorIdAsync(id);
            if (medida is null) return;

            medida.Eliminado = true;
            medida.FechaEliminacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}