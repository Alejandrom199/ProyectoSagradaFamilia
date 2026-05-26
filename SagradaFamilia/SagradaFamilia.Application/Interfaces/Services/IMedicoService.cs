using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IMedicoService
    {
        Task<IEnumerable<MedicoDto.ListResponse>> ObtenerTodosAsync();
        Task<PagedResponse<MedicoDto.ListResponse>> ObtenerPaginadoAsync(int page, int pageSize, string? search, string? sortBy, bool ascending);

        Task<MedicoDto.DetailResponse> ObtenerPorIdAsync(int id);

        Task<MedicoDto.DetailResponse> ObtenerPorUsuarioIdAsync(int usuarioId);

        Task<MedicoDto.DetailResponse> ObtenerConPacientesAsync(int id);

        Task<MedicoDto.DetailResponse> CrearAsync(MedicoDto.Create request);
        Task<MedicoDto.DetailResponse> ActualizarAsync(int id, MedicoDto.Update request);
        Task EliminarAsync(int id);
        Task RestablecerPasswordAsync(int medicoId);
    }
}