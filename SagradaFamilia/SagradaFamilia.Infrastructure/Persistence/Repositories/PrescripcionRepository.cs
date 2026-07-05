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
                .Include(p => p.Consulta)
                .Include(p => p.Medicamentos)
                .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado);

        public async Task<IEnumerable<Prescripcion>> ObtenerHistorialPorNinoAsync(int ninoId) =>
            await _context.Prescripciones
                .Include(p => p.Medico)
                .Include(p => p.Consulta)
                .Include(p => p.Medicamentos)
                .Where(p => p.NinoId == ninoId && !p.Eliminado)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

        public async Task<IEnumerable<Prescripcion>> ObtenerPorMedicoAsync(int medicoId) =>
            await _context.Prescripciones
                .Include(p => p.Nino)
                .Include(p => p.Consulta)
                .Include(p => p.Medicamentos)
                .Where(p => p.MedicoId == medicoId && !p.Eliminado)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

        public async Task<(IEnumerable<Prescripcion> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(
            int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending)
        {
            var query = _context.Prescripciones
                .Include(p => p.Medico)
                .Include(p => p.Consulta)
                .Include(p => p.Medicamentos)
                .Where(p => p.NinoId == ninoId && !p.Eliminado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(p =>
                    p.Medicamentos.Any(m => m.Nombre.ToLower().Contains(term)) ||
                    (p.Indicaciones != null && p.Indicaciones.ToLower().Contains(term)) ||
                    p.Medico.Nombre.ToLower().Contains(term) ||
                    p.Medico.Apellido.ToLower().Contains(term));
            }

            query = sortBy?.ToLower() switch
            {
                "nombremedico"  => ascending ? query.OrderBy(p => p.Medico.Apellido) : query.OrderByDescending(p => p.Medico.Apellido),
                _               => query.OrderByDescending(p => p.Id)
            };

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

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
