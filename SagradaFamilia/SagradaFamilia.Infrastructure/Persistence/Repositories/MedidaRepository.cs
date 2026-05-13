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

        public async Task<bool> ExisteMedidaEnMesAsync(int ninoId, int mes, int anio) =>
            await _context.Medidas
                .AnyAsync(m => m.NinoId == ninoId
                            && !m.Eliminado
                            && m.FechaMedicion.Year == anio
                            && m.FechaMedicion.Month == mes);

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

            _context.Medidas.Remove(medida);
            await _context.SaveChangesAsync();
        }
    }
}