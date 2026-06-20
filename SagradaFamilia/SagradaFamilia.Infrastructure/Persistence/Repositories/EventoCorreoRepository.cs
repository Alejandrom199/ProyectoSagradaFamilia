using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class EventoCorreoRepository : IEventoCorreoRepository
    {
        private readonly AppDbContext _context;

        public EventoCorreoRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<EventoCorreo>> ObtenerTodosConPlantillaAsync() =>
            await _context.EventosCorreo
                .Include(e => e.Plantilla)
                .OrderBy(e => e.Nombre)
                .ToListAsync();

        public async Task<EventoCorreo?> ObtenerPorCodigoConPlantillaAsync(string codigo) =>
            await _context.EventosCorreo
                .Include(e => e.Plantilla)
                .FirstOrDefaultAsync(e => e.Codigo == codigo);

        public async Task<bool> EstaEnUsoAsync(int plantillaId) =>
            await _context.EventosCorreo
                .AnyAsync(e => e.PlantillaCorreoId == plantillaId);

        public async Task ActualizarPlantillaAsync(int eventoId, int? plantillaId)
        {
            var evento = await _context.EventosCorreo.FindAsync(eventoId);
            if (evento is null) return;

            evento.PlantillaCorreoId = plantillaId;
            await _context.SaveChangesAsync();
        }
    }
}
