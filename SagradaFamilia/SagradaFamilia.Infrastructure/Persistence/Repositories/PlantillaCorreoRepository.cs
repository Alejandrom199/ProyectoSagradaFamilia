using Microsoft.EntityFrameworkCore;
using SagradaFamilia.Application.Interfaces.Repositories;
using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Repositories
{
    public class PlantillaCorreoRepository : IPlantillaCorreoRepository
    {
        private readonly AppDbContext _context;

        public PlantillaCorreoRepository(AppDbContext context) => _context = context;

        public async Task<IEnumerable<PlantillaCorreo>> ObtenerTodosAsync() =>
            await _context.PlantillasCorreo
                .Where(p => !p.Eliminado)
                .OrderBy(p => p.Codigo)
                .ToListAsync();

        public async Task<PlantillaCorreo?> ObtenerPorIdAsync(int id) =>
            await _context.PlantillasCorreo
                .FirstOrDefaultAsync(p => p.Id == id && !p.Eliminado);

        public async Task<PlantillaCorreo?> ObtenerPorCodigoAsync(string codigo) =>
            await _context.PlantillasCorreo
                .FirstOrDefaultAsync(p => p.Codigo == codigo && p.Activo && !p.Eliminado);

        public async Task<PlantillaCorreo> CrearAsync(PlantillaCorreo plantilla)
        {
            _context.PlantillasCorreo.Add(plantilla);
            await _context.SaveChangesAsync();
            return plantilla;
        }

        public async Task<PlantillaCorreo> ActualizarAsync(PlantillaCorreo plantilla)
        {
            _context.PlantillasCorreo.Update(plantilla);
            await _context.SaveChangesAsync();
            return plantilla;
        }

        public async Task EliminarAsync(int id)
        {
            var plantilla = await ObtenerPorIdAsync(id);
            if (plantilla is null) return;

            plantilla.Eliminado = true;
            plantilla.FechaEliminacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
