using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<IEnumerable<UsuarioDto.ListResponse>> ObtenerTodosAsync();
        Task<PagedResponse<UsuarioDto.ListResponse>> ObtenerPaginadoAsync(int page, int pageSize, string? search, string? sortBy, bool ascending);

        Task<UsuarioDto.DetailResponse> ObtenerPorIdAsync(int id);

        Task<IEnumerable<UsuarioDto.ListResponse>> ObtenerPorRolIdAsync(int rolId);

        Task<UsuarioDto.DetailResponse> CrearAsync(UsuarioDto.Create request);

        Task<UsuarioDto.DetailResponse> ActualizarAsync(int id, UsuarioDto.Update request);

        Task ActualizarEstadoAsync(int id, bool activo, int currentUserId);

        Task EliminarAsync(int id);

        Task<byte[]> GenerarPlantillaAsync();
        Task<byte[]> ExportarExcelAsync();
        Task<UsuarioDto.ImportResultado> ImportarAsync(Stream archivoStream);
    }
}