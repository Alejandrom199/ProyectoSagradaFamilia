using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Interfaces.Repositories
{
    public interface IPlantillaCorreoRepository
    {
        Task<IEnumerable<PlantillaCorreo>> ObtenerTodosAsync();
        Task<PlantillaCorreo?> ObtenerPorIdAsync(int id);
        Task<PlantillaCorreo?> ObtenerPorCodigoAsync(string codigo);
        Task<PlantillaCorreo> CrearAsync(PlantillaCorreo plantilla);
        Task<PlantillaCorreo> ActualizarAsync(PlantillaCorreo plantilla);
        Task EliminarAsync(int id);
    }
}
