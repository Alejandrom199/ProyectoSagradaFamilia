using SagradaFamilia.Domain.Entities;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IUsuarioEstadoHistorialRepository
    {
        Task<IEnumerable<UsuarioEstadoHistorial>> ObtenerPorUsuarioAsync(int usuarioId);
        Task RegistrarAsync(UsuarioEstadoHistorial registro);
    }
}
