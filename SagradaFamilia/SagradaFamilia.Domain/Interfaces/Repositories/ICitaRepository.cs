using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface ICitaRepository
    {
        Task<Cita?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Cita>> ObtenerPorNinoIdAsync(int ninoId);
        Task<IEnumerable<Cita>> ObtenerPorMedicoIdAsync(int medicoId, DateOnly fecha);
        Task<IEnumerable<Cita>> ObtenerPendientesPorMedicoAsync(int medicoId);
        Task<IEnumerable<Cita>> ObtenerPorPadreIdAsync(int padreId);
        Task<Cita> CrearAsync(Cita cita);
        Task<Cita> ActualizarAsync(Cita cita);
        Task EliminarAsync(int id);
    }
}