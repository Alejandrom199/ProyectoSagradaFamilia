using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class AlimentoRepository : IAlimentoRepository
    {
        private readonly AppDbContext _context;

        public AlimentoRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<Alimento>> ObtenerTodosAsync() =>
            await _context.Alimentos
                .Include(a => a.Categoria)
                .Where(a => !a.Eliminado)
                .OrderBy(a => a.Categoria.Nombre)
                .ThenBy(a => a.Nombre)
                .ToListAsync();

        public async Task<Alimento?> ObtenerPorIdAsync(int id) =>
            await _context.Alimentos
                .Include(a => a.Categoria)
                .FirstOrDefaultAsync(a => a.Id == id && !a.Eliminado);

        public async Task<IEnumerable<Alimento>> ObtenerPorRangoEdadAsync(int edadMeses) =>
            await _context.Alimentos
                .Include(a => a.Categoria)
                .Where(a => !a.Eliminado
                         && a.Activo
                         && a.EdadMinimaMeses <= edadMeses) // Usa el nombre correcto de la propiedad
                .OrderBy(a => a.Categoria.Nombre)
                .ThenBy(a => a.Nombre)
                .ToListAsync();

        public async Task<IEnumerable<Alimento>> ObtenerPorCategoriaAsync(int categoriaId) =>
            await _context.Alimentos
                .Include(a => a.Categoria)
                .Where(a => a.CategoriaId == categoriaId && !a.Eliminado)
                .OrderBy(a => a.Nombre)
                .ToListAsync();

        public async Task<Alimento> CrearAsync(Alimento alimento)
        {
            _context.Alimentos.Add(alimento);
            await _context.SaveChangesAsync();
            return alimento;
        }

        public async Task<Alimento> ActualizarAsync(Alimento alimento)
        {
            _context.Alimentos.Update(alimento);
            await _context.SaveChangesAsync();
            return alimento;
        }

        public async Task EliminarAsync(int id)
        {
            var alimento = await ObtenerPorIdAsync(id);
            if (alimento is null) return;

            _context.Alimentos.Remove(alimento);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoriaAlimento>> ObtenerCategoriasAsync() =>
            await _context.CategoriasAlimentos
                .Include(c => c.Alimentos.Where(a => a.Activo && !a.Eliminado))
                .Where(c => !c.Eliminado)
                .OrderBy(c => c.Nombre)
                .ToListAsync();

        public async Task<CategoriaAlimento?> ObtenerCategoriaPorIdAsync(int id) =>
            await _context.CategoriasAlimentos
                .FirstOrDefaultAsync(c => c.Id == id && !c.Eliminado);

        public async Task<CategoriaAlimento> CrearCategoriaAsync(CategoriaAlimento categoria)
        {
            _context.CategoriasAlimentos.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task<CategoriaAlimento> ActualizarCategoriaAsync(CategoriaAlimento categoria)
        {
            _context.CategoriasAlimentos.Update(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task EliminarCategoriaAsync(int id)
        {
            var categoria = await ObtenerCategoriaPorIdAsync(id);
            if (categoria is null) return;

            _context.CategoriasAlimentos.Remove(categoria);
            await _context.SaveChangesAsync();
        }
    }
}