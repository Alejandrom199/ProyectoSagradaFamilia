using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Interfaces.Repositories;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class CatalogoValorRepository : ICatalogoValorRepository
    {
        private readonly AppDbContext _context;

        public CatalogoValorRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<CatalogoValor>> ObtenerTodosAsync() =>
            await _context.CatalogosValor
                .Where(c => !c.Eliminado)
                .OrderBy(c => c.Tipo)
                .ThenBy(c => c.Codigo)
                .ToListAsync();

        public async Task<IEnumerable<CatalogoValor>> ObtenerPorTipoAsync(string tipo) =>
            await _context.CatalogosValor
                .Where(c => c.Tipo == tipo && c.Activo && !c.Eliminado)
                .OrderBy(c => c.Codigo)
                .ToListAsync();

        public async Task<CatalogoValor?> ObtenerPorTipoYCodigoAsync(string tipo, string codigo) =>
            await _context.CatalogosValor
                .FirstOrDefaultAsync(c => c.Tipo == tipo && c.Codigo == codigo && !c.Eliminado);

        public async Task<CatalogoValor?> ObtenerPorIdAsync(int id) =>
            await _context.CatalogosValor
                .FirstOrDefaultAsync(c => c.Id == id && !c.Eliminado);

        public async Task<CatalogoValor> CrearAsync(CatalogoValor catalogoValor)
        {
            _context.CatalogosValor.Add(catalogoValor);
            await _context.SaveChangesAsync();
            return catalogoValor;
        }

        public async Task<CatalogoValor> ActualizarAsync(CatalogoValor catalogoValor)
        {
            _context.CatalogosValor.Update(catalogoValor);
            await _context.SaveChangesAsync();
            return catalogoValor;
        }

        public async Task EliminarAsync(int id)
        {
            var catalogoValor = await ObtenerPorIdAsync(id);
            if (catalogoValor is null) return;

            _context.CatalogosValor.Remove(catalogoValor);
            await _context.SaveChangesAsync();
        }
    }
}
