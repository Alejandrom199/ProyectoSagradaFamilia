using SagradaFamilia.Application.DTOs.Auth;
using SagradaFamilia.Domain.Enums;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuResponse>> ObtenerMenuPorUsuarioAsync(int usuarioId, int rolId);
        Task AsignarPermisoAsync(AsignarPermisoRequest request);
        Task RevocarPermisoAsync(int usuarioId, int opcionAccionId);
    }
}
