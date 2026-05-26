using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IPadreRepository
    {
        Task<Padre?> ObtenerPorIdAsync(int id);
        Task<Padre?> ObtenerPorUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Padre>> ObtenerTodosAsync();
        Task<(IEnumerable<Padre> Items, int TotalItems)> ObtenerPaginadoAsync(int page, int pageSize, string? search, string? sortBy, bool ascending);
        Task<Padre> CrearAsync(Padre padre);
        Task<Padre> ActualizarAsync(Padre padre);
        Task EliminarAsync(int id);
    }
}
