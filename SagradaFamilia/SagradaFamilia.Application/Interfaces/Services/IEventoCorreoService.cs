using SagradaFamilia.Application.DTOs;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IEventoCorreoService
    {
        Task<IEnumerable<EventoCorreoDto.Response>> ObtenerTodosAsync();
        Task<EventoCorreoDto.Response> AsignarPlantillaAsync(int eventoId, int? plantillaId);
    }
}
