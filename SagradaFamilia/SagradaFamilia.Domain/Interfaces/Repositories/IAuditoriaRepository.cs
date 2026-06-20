using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IAuditoriaRepository
    {
        Task<IEnumerable<Auditoria>> ObtenerRecientesAsync(int top = 100);
        Task<IEnumerable<Auditoria>> ObtenerPorTablaAsync(string nombreTabla, string? clavePrimaria = null);
        Task<IEnumerable<Auditoria>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<(IEnumerable<Auditoria> Items, int TotalItems)> ObtenerPaginadoAsync(int page, int pageSize, string? search, string? sortBy, bool ascending);
        Task RegistrarAsync(Auditoria auditoria);

        // ── Dashboard del administrador ───────────────────────────
        Task<int>                    ObtenerConteoHoyAsync();
        Task<IEnumerable<DateTime>>  ObtenerFechasUltimosDiasAsync(int dias);
    }
}