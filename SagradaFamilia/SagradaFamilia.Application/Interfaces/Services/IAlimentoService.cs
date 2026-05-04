using SagradaFamilia.Application.DTOs.Alimentos;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IAlimentoService
    {
        Task<IEnumerable<AlimentoResponse>> ObtenerPorEdadAsync(int edadMeses);
        Task<IEnumerable<AlimentoResponse>> ObtenerTodosAsync();
        Task<AlimentoResponse> CrearAsync(CrearAlimentoRequest request);
        Task<AlimentoResponse> ActualizarAsync(int id, CrearAlimentoRequest request);
        Task EliminarAsync(int id);
        Task<IEnumerable<CategoriaResponse>> ObtenerCategoriasAsync();
        Task<CategoriaResponse> CrearCategoriaAsync(CrearCategoriaRequest request);
    }
}
