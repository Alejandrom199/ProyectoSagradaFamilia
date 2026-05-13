// En SagradaFamilia.Domain.Interfaces.Repositories
using SagradaFamilia.Domain.Entities;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task<IEnumerable<Usuario>> ObtenerTodosAsync(); 
    Task<IEnumerable<Usuario>> ObtenerPorRolIdAsync(int rolId);
    Task<Usuario?> ObtenerConRolesYPermisosAsync(int id);
    Task<bool> ExisteEmailAsync(string email);
    Task<Usuario> CrearAsync(Usuario usuario);
    Task<Usuario> ActualizarAsync(Usuario usuario);
    Task EliminarAsync(int id);
}