using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class PrescripcionRepository : IPrescripcionRepository
    {
        private readonly AppDbContext _context;

        public PrescripcionRepository(AppDbContext context) => _context = context;

        public async Task<Prescripcion?> ObtenerPorIdAsync(int id) =>
            await _context.Prescripciones
                .Include(p => p.Nino)
                .Include(p => p.Medico)
                .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado);

        public async Task<IEnumerable<Prescripcion>> ObtenerHistorialPorNinoAsync(int ninoId) =>
            await _context.Prescripciones
                .Include(p => p.Medico)
                .Where(p => p.NinoId == ninoId && !p.Eliminado)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

        public async Task<IEnumerable<Prescripcion>> ObtenerPorMedicoAsync(int medicoId) =>
            await _context.Prescripciones
                .Include(p => p.Nino)
                .Where(p => p.MedicoId == medicoId && !p.Eliminado)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

        public async Task<Prescripcion> CrearAsync(Prescripcion prescripcion)
        {
            _context.Prescripciones.Add(prescripcion);
            await _context.SaveChangesAsync();
            return prescripcion;
        }

        public async Task<Prescripcion> ActualizarAsync(Prescripcion prescripcion)
        {
            _context.Prescripciones.Update(prescripcion);
            await _context.SaveChangesAsync();
            return prescripcion;
        }

        public async Task EliminarAsync(int id)
        {
            var prescripcion = await ObtenerPorIdAsync(id);
            if (prescripcion is null) return;

            _context.Prescripciones.Remove(prescripcion);
            await _context.SaveChangesAsync();
        }
    }
}