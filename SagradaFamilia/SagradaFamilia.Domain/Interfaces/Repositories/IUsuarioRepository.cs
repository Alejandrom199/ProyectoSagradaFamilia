using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorIdAsync(int id);
        Task<Usuario?> ObtenerPorEmailAsync(string email);
        Task<IEnumerable<Usuario>> ObtenerPorRolIdAsync(int rolId);
        Task<bool> ExisteEmailAsync(string email);
        Task<Usuario> CrearAsync(Usuario usuario);
        Task<Usuario> ActualizarAsync(Usuario usuario);
        Task EliminarAsync(int id);
    }
}
