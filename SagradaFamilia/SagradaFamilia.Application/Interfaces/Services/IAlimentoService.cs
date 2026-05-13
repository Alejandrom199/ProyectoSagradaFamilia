using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IAlimentoService
    {
        Task<IEnumerable<AlimentoDto.Response>> ObtenerTodosAsync();
        Task<AlimentoDto.Response> ObtenerPorIdAsync(int id);

        Task<IEnumerable<AlimentoDto.Response>> ObtenerPorRangoEdadAsync(int edadMeses);
        Task<IEnumerable<AlimentoDto.Response>> ObtenerPorCategoriaAsync(int categoriaId);

        Task<AlimentoDto.Response> CrearAsync(AlimentoDto.Create request);
        Task<AlimentoDto.Response> ActualizarAsync(int id, AlimentoDto.Update request);
        Task EliminarAsync(int id);

        Task<IEnumerable<CategoriaDto.Response>> ObtenerCategoriasAsync();
        Task<CategoriaDto.Response> ObtenerCategoriaPorIdAsync(int id);

        Task<CategoriaDto.Response> CrearCategoriaAsync(CategoriaDto.Create request);
        Task<CategoriaDto.Response> ActualizarCategoriaAsync(int id, CategoriaDto.Update request);
        Task EliminarCategoriaAsync(int id);
    }
}