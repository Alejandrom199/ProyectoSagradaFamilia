using SagradaFamilia.Application.DTOs.Ninos;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface INinoService
    {
        Task<NinoResponse> ObtenerPorIdAsync(int id);
        Task<IEnumerable<NinoResponse>> ObtenerTodosAsync();
        Task<IEnumerable<NinoResponse>> ObtenerPorRepresentanteAsync(int representanteId);
        Task<NinoResponse> CrearAsync(CrearNinoRequest request);
        Task<NinoResponse> ActualizarAsync(int id, ActualizarNinoRequest request);
        Task EliminarAsync(int id);
    }
}
