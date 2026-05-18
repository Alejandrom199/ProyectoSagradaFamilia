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