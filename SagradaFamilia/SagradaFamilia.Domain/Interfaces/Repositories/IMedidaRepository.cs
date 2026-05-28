using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Application.Interfaces.Repositories
{
    public interface IMedidaRepository
    {
        Task<Medida?> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Medida>> ObtenerTodosAsync();
        Task<IEnumerable<Medida>> ObtenerPorNinoAsync(int ninoId);
        Task<Medida?> ObtenerUltimaMedidaAsync(int ninoId);
        Task<Medida?> ObtenerPorNinoYMesAsync(int ninoId, int mes, int anio);
        Task<bool> ExisteMedidaEnMesAsync(int ninoId, int mes, int anio);

        Task<(IEnumerable<Medida> Items, int TotalItems)> ObtenerPaginadoPorNinoAsync(int ninoId, int page, int pageSize, string? search, string? sortBy, bool ascending);
        Task<Medida> CrearAsync(Medida medida);
        Task<Medida> ActualizarAsync(Medida medida);
        Task EliminarAsync(int id);
    }
}
