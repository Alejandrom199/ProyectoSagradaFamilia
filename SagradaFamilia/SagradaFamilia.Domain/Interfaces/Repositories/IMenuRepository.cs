using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Domain.Interfaces.Repositories
{
    public interface IMenuRepository
    {
        Task<IEnumerable<Modulo>> ObtenerModulosActivosAsync();
        Task<Modulo?> ObtenerModuloPorIdAsync(int id);
        Task<Opcion?> ObtenerOpcionPorIdAsync(int id);
        Task<OpcionAccion?> ObtenerOpcionAccionAsync(int opcionId, int accionId);

        Task<IEnumerable<RolPermiso>> ObtenerPermisosPorRolIdAsync(int rolId);
        Task<IEnumerable<UsuarioPermiso>> ObtenerPermisosPorUsuarioAsync(int usuarioId);
        
        Task<UsuarioPermiso> CrearUsuarioPermisoAsync(UsuarioPermiso permiso);
        Task EliminarUsuarioPermisoAsync(int usuarioId, int opcionAccionId);

        Task<IEnumerable<Modulo>> ObtenerMenuPorUsuarioAsync(int usuarioId, int rolId);
    }
}
