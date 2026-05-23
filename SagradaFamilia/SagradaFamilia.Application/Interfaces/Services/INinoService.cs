using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface INinoService
    {
        Task<NinoDto.DetailResponse> ObtenerPorIdAsync(int id);
        Task<IEnumerable<NinoDto.ListResponse>> ObtenerTodosAsync();

        Task<IEnumerable<NinoDto.ListResponse>> ObtenerPorPadreIdAsync(int padreId);

        Task<IEnumerable<NinoDto.ListResponse>> ObtenerPorMedicoIdAsync(int medicoId);

        Task<bool> PerteneceAPadreAsync(int ninoId, int padreId);
        Task<bool> PerteneceAMedicoAsync(int ninoId, int medicoId);

        Task<NinoDto.DetailResponse> CrearAsync(NinoDto.Create request);
        Task<NinoDto.DetailResponse> ActualizarAsync(int id, NinoDto.Update request);
        Task EliminarAsync(int id);
        Task<IEnumerable<NinoDto.ListResponse>> ObtenerMisPorUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<NinoDto.ListResponse>> ObtenerMisPacientesPorUsuarioIdAsync(int usuarioId);
    }
}