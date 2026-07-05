using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class ConsultaRepository : IConsultaRepository
    {
        private readonly AppDbContext _context;

        public ConsultaRepository(AppDbContext context) => _context = context;

        public async Task<Consulta?> ObtenerPorIdAsync(int id) =>
            await _context.Consultas
                .Include(c => c.Nino)
                .Include(c => c.Medico)
                .Include(c => c.Cita)
                .Include(c => c.Prescripciones)
                    .ThenInclude(p => p.Medicamentos)
                .FirstOrDefaultAsync(c => c.Id == id && !c.Eliminado);

        public async Task<Consulta?> ObtenerPorCitaIdAsync(int citaId) =>
            await _context.Consultas
                .FirstOrDefaultAsync(c => c.CitaId == citaId && !c.Eliminado);

        public async Task<IEnumerable<Consulta>> ObtenerPorNinoIdAsync(int ninoId) =>
            await _context.Consultas
                .Where(c => c.NinoId == ninoId && !c.Eliminado)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();

        public async Task<Consulta> CrearAsync(Consulta consulta)
        {
            _context.Consultas.Add(consulta);
            await _context.SaveChangesAsync();
            return consulta;
        }

        public async Task<Consulta> ActualizarAsync(Consulta consulta)
        {
            _context.Consultas.Update(consulta);
            await _context.SaveChangesAsync();
            return consulta;
        }
    }
}
