using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IPadreRepository
    {
        Task<Padre?> ObtenerPorIdAsync(int id);
        Task<Padre?> ObtenerPorUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Padre>> ObtenerTodosAsync(int? medicoId = null);
        Task<IEnumerable<Padre>> ObtenerPorMedicoIdAsync(int medicoId);
        Task<bool> PerteneceAMedicoAsync(int padreId, int medicoId);
        Task<(IEnumerable<Padre> Items, int TotalItems)> ObtenerPaginadoAsync(int page, int pageSize, string? search, string? sortBy, bool ascending, int? medicoId = null);
        Task<Padre> CrearAsync(Padre padre);
        Task<Padre> ActualizarAsync(Padre padre);
        Task EliminarAsync(int id);
    }
}
