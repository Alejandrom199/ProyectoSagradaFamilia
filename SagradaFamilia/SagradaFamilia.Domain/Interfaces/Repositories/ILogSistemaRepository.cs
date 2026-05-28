using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface ILogSistemaRepository
    {
        Task<IEnumerable<LogSistema>> ObtenerRecientesAsync(int top = 100);
        Task<IEnumerable<LogSistema>> ObtenerErroresRecientesAsync(int top);
        Task<IEnumerable<LogSistema>> ObtenerPorNivelAsync(string nivel);

        Task<(IEnumerable<LogSistema> Items, int TotalItems)> ObtenerPaginadoAsync(int page, int pageSize, string? search, string? sortBy, bool ascending);
        Task RegistrarAsync(LogSistema log);
    }
}