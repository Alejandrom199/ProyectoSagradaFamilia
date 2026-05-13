using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IAuditoriaRepository
    {
        Task<IEnumerable<Auditoria>> ObtenerPorTablaAsync(string nombreTabla, string clavePrimaria);
        Task<IEnumerable<Auditoria>> ObtenerPorUsuarioAsync(int usuarioId);

        Task RegistrarAsync(Auditoria auditoria);
    }
}