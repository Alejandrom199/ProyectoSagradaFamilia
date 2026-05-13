using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDto.ListResponse>> ObtenerTodosAsync();

        Task<UsuarioDto.DetailResponse> ObtenerPorIdAsync(int id);

        Task<IEnumerable<UsuarioDto.ListResponse>> ObtenerPorRolIdAsync(int rolId);

        Task<UsuarioDto.DetailResponse> CrearAsync(UsuarioDto.Create request);

        Task<UsuarioDto.DetailResponse> ActualizarAsync(int id, UsuarioDto.Update request);

        Task ActualizarEstadoAsync(int id, bool activo);

        Task EliminarAsync(int id);
    }
}