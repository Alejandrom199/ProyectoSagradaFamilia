using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuDto.MenuResponse>> ObtenerMenuPorUsuarioAsync(int usuarioId, int rolId);
        Task AsignarPermisoAsync(PermisoDto.AsignarRequest request);
        Task RevocarPermisoAsync(int usuarioId, int opcionAccionId);
        Task<IEnumerable<PermisoDto.ModuloPermisoResponse>> ObtenerPermisosRolAsync(int rolId);
        Task ActualizarPermisoRolAsync(int rolId, int opcionAccionId, bool permitido);
    }
}