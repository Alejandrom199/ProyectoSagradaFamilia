using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Common;
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
                        .ThenInclude(p => p.Usuario)
                .Include(c => c.Medico)
                .Include(c => c.Consulta)
                    .ThenInclude(co => co!.Prescripciones)
                        .ThenInclude(p => p.Medicamentos)
                .FirstOrDefaultAsync(c => c.Id == id && !c.Eliminado);

        public async Task<IEnumerable<Cita>> ObtenerHistorialPorMedicoAsync(int usuarioId) =>
            await _context.Citas
                .Include(c => c.Nino)
                .Include(c => c.Medico)
                .Include(c => c.Consulta)
                    .ThenInclude(co => co!.Prescripciones)
                        .ThenInclude(p => p.Medicamentos)
                .Where(c => c.Medico.UsuarioId == usuarioId && !c.Eliminado)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync();

        public async Task<IEnumerable<Cita>> ObtenerPorNinoIdAsync(int ninoId) =>
            await _context.Citas
                .Include(c => c.Medico)
                .Where(c => c.NinoId == ninoId && !c.Eliminado)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync();

        public async Task<IEnumerable<Cita>> ObtenerPorMedicoIdAsync(int usuarioId, DateOnly fecha)
        {
            var inicio = fecha.ToDateTime(TimeOnly.MinValue);
            var fin = fecha.ToDateTime(TimeOnly.MaxValue);

            return await _context.Citas
                .Include(c => c.Nino)
                .Where(c => c.Medico.UsuarioId == usuarioId
                         && !c.Eliminado
                         && c.FechaHora >= inicio
                         && c.FechaHora <= fin)
                .OrderBy(c => c.FechaHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerPendientesPorMedicoAsync(int usuarioId) =>
            await _context.Citas
                .Include(c => c.Nino)
                .Include(c => c.Medico) 
                .Where(c => c.Medico.UsuarioId == usuarioId
                         && c.Estado == EstadoCita.Pendiente
                         && !c.Eliminado
                         && c.FechaHora >= RelojEcuador.Hoy)
                .OrderBy(c => c.FechaHora)
                .ToListAsync();

        public async Task<(IEnumerable<Cita> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(
            int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending)
        {
            var query = _context.Citas
                .Include(c => c.Medico)
                .Where(c => c.NinoId == ninoId && !c.Eliminado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(c =>
                    (c.Motivo != null && c.Motivo.ToLower().Contains(term)) ||
                    c.Medico.Nombre.ToLower().Contains(term) ||
                    c.Medico.Apellido.ToLower().Contains(term));
            }

            query = sortBy?.ToLower() switch
            {
                "fechahora"   => ascending ? query.OrderBy(c => c.FechaHora)           : query.OrderByDescending(c => c.FechaHora),
                "nombremedico"=> ascending ? query.OrderBy(c => c.Medico.Apellido)     : query.OrderByDescending(c => c.Medico.Apellido),
                "motivo"      => ascending ? query.OrderBy(c => c.Motivo)              : query.OrderByDescending(c => c.Motivo),
                "estado"      => ascending ? query.OrderBy(c => c.Estado)              : query.OrderByDescending(c => c.Estado),
                _             => query.OrderByDescending(c => c.FechaHora)
            };

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

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

        public async Task<bool> ExisteTraslapeAsync(
            int medicoId, DateTime inicio, DateTime fin, int? excluirCitaId = null)
        {
            return await _context.Citas
                .Where(c => c.MedicoId == medicoId
                         && !c.Eliminado
                         && c.Estado != EstadoCita.Cancelada
                         && c.Estado != EstadoCita.NoAsistio
                         && c.Estado != EstadoCita.Reagendada
                         && (excluirCitaId == null || c.Id != excluirCitaId)
                         && c.FechaHora < fin
                         && (c.FechaHoraFin.HasValue
                                ? c.FechaHoraFin.Value > inicio
                                : c.FechaHora.AddMinutes(30) > inicio))
                .AnyAsync();
        }

        public async Task<IEnumerable<Cita>> ObtenerPorPadreIdAsync(int padreId) =>
            await _context.Citas
                .Include(c => c.Nino)
                .Include(c => c.Medico)
                .Where(c => c.Nino.PadreId == padreId && !c.Eliminado)
                .OrderByDescending(c => c.FechaHora)
                .ToListAsync();


    }
}