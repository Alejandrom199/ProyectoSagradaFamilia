using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class NinoRepository : INinoRepository
    {
        private readonly AppDbContext _context;

        public NinoRepository(AppDbContext context) => _context = context;

        public async Task<Nino?> ObtenerPorIdAsync(int id) =>
            await _context.Ninos
                .Include(n => n.Representante)
                .FirstOrDefaultAsync(n => n.Id == id && !n.Eliminado);

        public async Task<IEnumerable<Nino>> ObtenerTodosAsync() =>
            await _context.Ninos
                .Include(n => n.Representante)
                .Where(n => !n.Eliminado)
                .OrderBy(n => n.Apellido)
                .ThenBy(n => n.Nombre)
                .ToListAsync();

        public async Task<IEnumerable<Nino>> ObtenerPorRepresentanteAsync(int representanteId) =>
            await _context.Ninos
                .Include(n => n.Representante)
                .Where(n => n.RepresentanteId == representanteId && !n.Eliminado)
                .OrderBy(n => n.Nombre)
                .ToListAsync();

        public async Task<bool> PerteneceARepresentanteAsync(int ninoId, int representanteId) =>
            await _context.Ninos
                .AnyAsync(n => n.Id == ninoId
                            && n.RepresentanteId == representanteId
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

            nino.Eliminado = true;
            nino.FechaEliminacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
