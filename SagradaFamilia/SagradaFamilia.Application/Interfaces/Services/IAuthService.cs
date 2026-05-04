using SagradaFamilia.Application.DTOs.Auth;

namespace SagradaFamilia.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<UsuarioResponse> CrearPadreAsync(CrearPadreRequest request);
        Task<IEnumerable<UsuarioResponse>> ObtenerPadresAsync();
        Task EliminarPadreAsync(int id);
    }
}
