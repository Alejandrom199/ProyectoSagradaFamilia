using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IPrescripcionService
    {
        Task<PrescripcionDto.Response> ObtenerPorIdAsync(int id);

        Task<IEnumerable<PrescripcionDto.Response>> ObtenerHistorialPorNinoAsync(int ninoId);
        Task<(IEnumerable<PrescripcionDto.Response> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending);

        Task<IEnumerable<PrescripcionDto.Response>> ObtenerPorMedicoAsync(int medicoId);

        Task<PrescripcionDto.Response> CrearAsync(PrescripcionDto.Create request, int medicoId);

        Task<PrescripcionDto.Response> ActualizarAsync(int id, PrescripcionDto.Update request);
        Task EliminarAsync(int id);
    }
}