using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IPlantillaCorreoService
    {
        Task<IEnumerable<PlantillaCorreoDto.Response>> ObtenerTodosAsync();
        Task<PlantillaCorreoDto.Response> ObtenerPorIdAsync(int id);
        Task<PlantillaCorreoDto.Response> CrearAsync(PlantillaCorreoDto.Create request);
        Task<PlantillaCorreoDto.Response> ActualizarAsync(int id, PlantillaCorreoDto.Update request);
        Task EliminarAsync(int id);
    }
}
