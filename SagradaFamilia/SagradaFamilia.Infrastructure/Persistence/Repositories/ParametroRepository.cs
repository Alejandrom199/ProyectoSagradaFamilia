using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class ParametroRepository : IParametroRepository
    {
        private readonly AppDbContext _context;

        public ParametroRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<ParametroSistema>> ObtenerTodosAsync() =>
            await _context.ParametrosSistema
                .Where(p => !p.Eliminado)
                .OrderBy(p => p.Grupo)
                .ThenBy(p => p.Codigo)
                .ToListAsync();

        public async Task<IEnumerable<ParametroSistema>> ObtenerPorGrupoAsync(string grupo) =>
            await _context.ParametrosSistema
                .Where(p => p.Grupo == grupo && !p.Eliminado)
                .OrderBy(p => p.Codigo)
                .ToListAsync();

        public async Task<ParametroSistema?> ObtenerPorGrupoYCodigoAsync(string grupo, string codigo) =>
            await _context.ParametrosSistema
                .FirstOrDefaultAsync(p => p.Grupo == grupo && p.Codigo == codigo && !p.Eliminado);

        public async Task<ParametroSistema?> ObtenerPorIdAsync(int id) =>
            await _context.ParametrosSistema
                .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado);

        public async Task<ParametroSistema> CrearAsync(ParametroSistema parametro)
        {
            _context.ParametrosSistema.Add(parametro);
            await _context.SaveChangesAsync();
            return parametro;
        }

        public async Task<ParametroSistema> ActualizarAsync(ParametroSistema parametro)
        {
            _context.ParametrosSistema.Update(parametro);
            await _context.SaveChangesAsync();
            return parametro;
        }

        public async Task EliminarAsync(int id)
        {
            var parametro = await ObtenerPorIdAsync(id);
            if (parametro is null) return;

            _context.ParametrosSistema.Remove(parametro);
            await _context.SaveChangesAsync();
        }
    }
}
