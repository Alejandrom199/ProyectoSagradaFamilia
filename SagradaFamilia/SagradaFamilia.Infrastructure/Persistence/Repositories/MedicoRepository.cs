using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly AppDbContext _context;

        public MedicoRepository(AppDbContext context) => _context = context;

        public async Task<Medico?> ObtenerPorIdAsync(int id) =>
            await _context.Medicos
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id && !m.Eliminado);

        public async Task<Medico?> ObtenerPorUsuarioIdAsync(int usuarioId) =>
            await _context.Medicos
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioId == usuarioId && !m.Eliminado);

        public async Task<IEnumerable<Medico>> ObtenerTodosAsync() =>
            await _context.Medicos
                .Include(m => m.Usuario)
                .Where(m => !m.Eliminado)
                .OrderBy(m => m.Apellido)
                .ThenBy(m => m.Nombre)
                .ToListAsync();

        public async Task<Medico?> ObtenerConPacientesAsync(int id) =>
            await _context.Medicos
                .Include(m => m.Usuario)
                .Include(m => m.Ninos.Where(n => !n.Eliminado))
                    .ThenInclude(n => n.Padre) 
                .FirstOrDefaultAsync(m => m.Id == id && !m.Eliminado);

        public async Task<Medico> CrearAsync(Medico medico)
        {
            _context.Medicos.Add(medico);
            await _context.SaveChangesAsync();
            return medico;
        }

        public async Task<Medico> ActualizarAsync(Medico medico)
        {
            _context.Medicos.Update(medico);
            await _context.SaveChangesAsync();
            return medico;
        }

        public async Task EliminarAsync(int id)
        {
            var medico = await ObtenerPorIdAsync(id);
            if (medico is null) return;

            _context.Medicos.Remove(medico);
            await _context.SaveChangesAsync();
        }
    }
}