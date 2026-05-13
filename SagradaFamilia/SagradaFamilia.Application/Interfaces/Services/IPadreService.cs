using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IPadreService
    {
        Task<IEnumerable<PadreDto.ListResponse>> ObtenerTodosAsync();

        Task<PadreDto.DetailResponse> ObtenerPorIdAsync(int id);
        Task<PadreDto.DetailResponse> ObtenerPorUsuarioIdAsync(int usuarioId);

        Task<PadreDto.DetailResponse> CrearAsync(PadreDto.Create request);
        Task<PadreDto.DetailResponse> ActualizarAsync(int id, PadreDto.Update request);
        Task EliminarAsync(int id);
    }
}