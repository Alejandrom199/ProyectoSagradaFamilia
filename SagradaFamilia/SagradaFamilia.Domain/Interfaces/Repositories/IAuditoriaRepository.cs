using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IAuditoriaRepository
    {
        Task<IEnumerable<Auditoria>> ObtenerRecientesAsync(int top = 100);
        Task<IEnumerable<Auditoria>> ObtenerPorTablaAsync(string nombreTabla, string? clavePrimaria = null);
        Task<IEnumerable<Auditoria>> ObtenerPorUsuarioAsync(int usuarioId);
        Task RegistrarAsync(Auditoria auditoria);
    }
}