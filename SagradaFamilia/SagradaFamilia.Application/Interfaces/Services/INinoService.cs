using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Application.DTOs.Common;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface INinoService
    {
        Task<NinoDto.DetailResponse> ObtenerPorIdAsync(int id);
        Task<IEnumerable<NinoDto.ListResponse>> ObtenerTodosAsync();
        Task<PagedResponse<NinoDto.ListResponse>> ObtenerPaginadoAsync(int page, int pageSize, string? search, string? sortBy, bool ascending, int? medicoId = null);

        Task<IEnumerable<NinoDto.ListResponse>> ObtenerPorPadreIdAsync(int padreId);

        Task<IEnumerable<NinoDto.ListResponse>> ObtenerPorMedicoIdAsync(int medicoId);

        Task<bool> PerteneceAPadreAsync(int ninoId, int padreId);
        Task<bool> PerteneceAMedicoAsync(int ninoId, int medicoId);

        Task<NinoDto.DetailResponse> CrearAsync(NinoDto.Create request);
        Task<NinoDto.DetailResponse> ActualizarAsync(int id, NinoDto.Update request);
        Task CambiarMedicoAsync(int ninoId, int nuevoMedicoId);
        Task CambiarPadreAsync(int ninoId, int nuevoPadreId);
        Task EliminarAsync(int id);
        Task<IEnumerable<NinoDto.ListResponse>> ObtenerMisPorUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<NinoDto.ListResponse>> ObtenerMisPacientesPorUsuarioIdAsync(int usuarioId);

        Task<byte[]> GenerarPlantillaAsync();
        Task<byte[]> ExportarExcelAsync(int? medicoId = null);
        Task<NinoDto.ImportResultado> ImportarAsync(Stream archivoStream, int medicoId);
    }
}