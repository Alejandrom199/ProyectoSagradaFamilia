using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class PadreRepository : IPadreRepository
    {
        private readonly AppDbContext _context;

        public PadreRepository(AppDbContext context) => _context = context;

        public async Task<Padre?> ObtenerPorIdAsync(int id) =>
            await _context.Padres
                .Include(p => p.Usuario)
                .Include(p => p.Medico) 
                .Include(p => p.Ninos.Where(n => !n.Eliminado)) 
                .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado);

        public async Task<Padre?> ObtenerPorUsuarioIdAsync(int usuarioId) =>
            await _context.Padres
                .Include(p => p.Usuario)
                .Include(p => p.Medico)
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId && !p.Eliminado);

        public async Task<IEnumerable<Padre>> ObtenerTodosAsync() =>
            await _context.Padres
                .Include(p => p.Usuario)
                .Include(p => p.Medico)
                .Where(p => !p.Eliminado)
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .ToListAsync();

        public async Task<Padre> CrearAsync(Padre padre)
        {
            _context.Padres.Add(padre);
            await _context.SaveChangesAsync();
            return padre;
        }

        public async Task<Padre> ActualizarAsync(Padre padre)
        {
            _context.Padres.Update(padre);
            await _context.SaveChangesAsync();
            return padre;
        }

        public async Task EliminarAsync(int id)
        {
            var padre = await ObtenerPorIdAsync(id);
            if (padre != null)
            {
                _context.Padres.Remove(padre);
                await _context.SaveChangesAsync();
            }
        }
    }
}