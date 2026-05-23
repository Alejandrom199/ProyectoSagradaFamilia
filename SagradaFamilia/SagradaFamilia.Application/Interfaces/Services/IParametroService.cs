using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IParametroService
    {
        Task<IEnumerable<ParametroDto.Response>> ObtenerTodosAsync();
        Task<IEnumerable<ParametroDto.Response>> ObtenerPorGrupoAsync(string grupo);
        Task<ParametroDto.Response?> ObtenerPorGrupoYCodigoAsync(string grupo, string codigo);
        Task<ParametroDto.Response> ObtenerPorIdAsync(int id);
        Task<ParametroDto.Response> CrearAsync(ParametroDto.Create request);
        Task<ParametroDto.Response> ActualizarAsync(int id, ParametroDto.Update request);
        Task EliminarAsync(int id);
    }
}
