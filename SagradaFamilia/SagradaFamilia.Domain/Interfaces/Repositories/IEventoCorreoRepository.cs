using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IEventoCorreoRepository
    {
        Task<IEnumerable<EventoCorreo>> ObtenerTodosConPlantillaAsync();
        Task<EventoCorreo?> ObtenerPorCodigoConPlantillaAsync(string codigo);
        Task<bool> EstaEnUsoAsync(int plantillaId);
        Task ActualizarPlantillaAsync(int eventoId, int? plantillaId);
    }
}
