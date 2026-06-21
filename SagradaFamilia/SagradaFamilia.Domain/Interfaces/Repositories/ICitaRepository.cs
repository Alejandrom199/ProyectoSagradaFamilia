using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface ICitaRepository
    {
        Task<Cita?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Cita>> ObtenerHistorialPorMedicoAsync(int usuarioId);
        Task<IEnumerable<Cita>> ObtenerPorNinoIdAsync(int ninoId);
        Task<IEnumerable<Cita>> ObtenerPorMedicoIdAsync(int medicoId, DateOnly fecha);
        Task<IEnumerable<Cita>> ObtenerPendientesPorMedicoAsync(int medicoId);
        Task<IEnumerable<Cita>> ObtenerPorPadreIdAsync(int padreId);
        Task<(IEnumerable<Cita> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending);
        Task<Cita> CrearAsync(Cita cita);
        Task<Cita> ActualizarAsync(Cita cita);
        Task EliminarAsync(int id);
        Task<bool> ExisteTraslapeAsync(int medicoId, DateTime inicio, DateTime fin, int? excluirCitaId = null);
    }
}