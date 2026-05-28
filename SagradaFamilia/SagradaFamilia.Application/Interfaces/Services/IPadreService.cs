using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IPadreService
    {
        Task<IEnumerable<PadreDto.ListResponse>> ObtenerTodosAsync();
        Task<PagedResponse<PadreDto.ListResponse>> ObtenerPaginadoAsync(int page, int pageSize, string? search, string? sortBy, bool ascending);

        Task<PadreDto.DetailResponse> ObtenerPorIdAsync(int id);
        Task<PadreDto.DetailResponse> ObtenerPorUsuarioIdAsync(int usuarioId);

        Task<PadreDto.DetailResponse> CrearAsync(PadreDto.Create request);
        Task<PadreDto.DetailResponse> ActualizarAsync(int id, PadreDto.Update request);
        Task EliminarAsync(int id);
        Task RestablecerPasswordAsync(int padreId);
        Task CambiarEmailAsync(int padreId, string nuevoEmail);

        Task<byte[]> GenerarPlantillaAsync();
        Task<byte[]> ExportarExcelAsync();
        Task<PadreDto.ImportResultado> ImportarAsync(Stream archivoStream, int medicoId);
    }
}