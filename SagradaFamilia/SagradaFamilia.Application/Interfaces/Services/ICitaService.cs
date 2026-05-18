using SagradaFamilia.Application.DTOs;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface ICitaService
    {
        Task<CitaDto.Response> ObtenerPorIdAsync(int id);

        Task<IEnumerable<CitaDto.Response>> ObtenerPorNinoIdAsync(int ninoId);

        Task<IEnumerable<CitaDto.Response>> ObtenerPorMedicoIdAsync(int medicoId, DateOnly fecha);

        Task<IEnumerable<CitaDto.Response>> ObtenerPendientesPorMedicoAsync(int medicoId);

        Task<CitaDto.Response> CrearAsync(CitaDto.Create request);
        Task<CitaDto.Response> ActualizarAsync(int id, CitaDto.Update request);

        Task ActualizarEstadoAsync(int id, EstadoCita nuevoEstado);

        Task EliminarAsync(int id);
        Task<IEnumerable<CitaDto.Response>> ObtenerPorPadreIdAsync(int padreId);
        Task<IEnumerable<CitaDto.Response>> ObtenerProximasPorMedicoAsync(int medicoId);
    }
}