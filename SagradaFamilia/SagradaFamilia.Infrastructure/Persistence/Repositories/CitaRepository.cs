using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class CitaRepository : ICitaRepository
    {
        private readonly AppDbContext _context;

        public CitaRepository(AppDbContext context) => _context = context;

        public async Task<Cita?> ObtenerPorIdAsync(int id) =>
            await _context.Citas
                .Include(c => c.Nino)
                    .ThenInclude(n => n.Padre)
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.Id == id && !c.Eliminado);

        public async Task<IEnumerable<Cita>> ObtenerPorNinoIdAsync(int ninoId) =>
            await _context.Citas
                .Include(c => c.Medico)
                .Where(c => c.NinoId == ninoId && !c.Eliminado)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync();

        public async Task<IEnumerable<Cita>> ObtenerPorMedicoIdAsync(int medicoId, DateOnly fecha) =>
            await _context.Citas
                .Include(c => c.Nino)
                .Where(c => c.MedicoId == medicoId
                         && !c.Eliminado
                         && c.FechaHora.Year == fecha.Year
                         && c.FechaHora.Month == fecha.Month
                         && c.FechaHora.Day == fecha.Day)
                .OrderBy(c => c.FechaHora)
                .ToListAsync();

        public async Task<IEnumerable<Cita>> ObtenerPendientesPorMedicoAsync(int medicoId) =>
            await _context.Citas
                .Include(c => c.Nino)
                .Where(c => c.MedicoId == medicoId
                         && c.Estado == EstadoCita.Pendiente
                         && !c.Eliminado
                         && c.FechaHora >= DateTime.UtcNow.Date)
                .OrderBy(c => c.FechaHora)
                .ToListAsync();

        public async Task<Cita> CrearAsync(Cita cita)
        {
            _context.Citas.Add(cita);
            await _context.SaveChangesAsync();
            return cita;
        }

        public async Task<Cita> ActualizarAsync(Cita cita)
        {
            _context.Citas.Update(cita);
            await _context.SaveChangesAsync();
            return cita;
        }

        public async Task EliminarAsync(int id)
        {
            var cita = await ObtenerPorIdAsync(id);
            if (cita is null) return;

            _context.Citas.Remove(cita);
            await _context.SaveChangesAsync();
        }
    }
}