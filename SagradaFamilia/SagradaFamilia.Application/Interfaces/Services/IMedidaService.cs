using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IMedidaService
    {
        Task<MedidaDto.Response> ObtenerPorIdAsync(int id);

        Task<IEnumerable<MedidaDto.Response>> ObtenerPorNinoAsync(int ninoId);
        Task<(IEnumerable<MedidaDto.Response> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending);

        Task<MedidaDto.Response?> ObtenerUltimaMedidaAsync(int ninoId);

        Task<MedidaDto.Response> CrearAsync(MedidaDto.Create request, int medicoId);

        Task<MedidaDto.Response> ActualizarAsync(int id, MedidaDto.Update request);
        Task EliminarAsync(int id);

        Task<byte[]> GenerarPlantillaAsync();
        Task<byte[]> ExportarExcelAsync();
        Task<MedidaDto.ImportResultado> ImportarAsync(Stream archivoStream, int medicoId);
    }
}